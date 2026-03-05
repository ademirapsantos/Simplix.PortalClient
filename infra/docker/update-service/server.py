import json
import os
import subprocess
import threading
import time
import urllib.error
import urllib.request
from datetime import datetime, timezone
from http.server import BaseHTTPRequestHandler, HTTPServer
from pathlib import Path


UPDATE_TOKEN = os.getenv("UPDATE_TOKEN", "")
REQUIRES_ADMIN_APPROVAL = os.getenv("UPDATE_REQUIRES_ADMIN_APPROVAL", "true").lower() == "true"
CHANNEL = os.getenv("UPDATE_CHANNEL", "stable")
ENVIRONMENT = os.getenv("UPDATE_ENVIRONMENT", "dev")
MANIFEST_BASE_URL = os.getenv("UPDATE_MANIFEST_BASE_URL", "").rstrip("/")

PROJECT_DIR = os.getenv("UPDATE_PROJECT_DIR", "/workspace")
ENV_FILE = os.getenv("UPDATE_ENV_FILE", "/workspace/.env.dev")
COMPOSE_FILE = os.getenv("UPDATE_COMPOSE_FILE", "/workspace/docker-compose.yml")
COMPOSE_OVERRIDE_FILE = os.getenv("UPDATE_COMPOSE_OVERRIDE_FILE", "")
APP_SERVICE_NAME = os.getenv("UPDATE_APP_SERVICE_NAME", "app")
APP_HEALTH_URL = os.getenv("UPDATE_APP_HEALTH_URL", "http://app:8080/health")
HEALTH_ATTEMPTS = int(os.getenv("UPDATE_HEALTH_ATTEMPTS", "15"))
HEALTH_SLEEP_SECONDS = int(os.getenv("UPDATE_HEALTH_SLEEP_SECONDS", "6"))

MANIFEST_PATH = Path(os.getenv("UPDATE_MANIFEST_PATH", "/app/manifest/update-manifest.json"))
DATA_DIR = Path(os.getenv("UPDATE_DATA_DIR", "/app/data"))
LOCK_FILE = DATA_DIR / "update.lock"
STATE_FILE = DATA_DIR / "update-state.json"

_state_lock = threading.Lock()
DATA_DIR.mkdir(parents=True, exist_ok=True)


def now_utc_iso():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")


def load_json(path: Path):
    if not path.exists():
        return {}
    try:
        with path.open("r", encoding="utf-8") as fh:
            payload = json.load(fh)
            return payload if isinstance(payload, dict) else {}
    except Exception:
        return {}


def write_json(path: Path, payload):
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8") as fh:
        json.dump(payload, fh, ensure_ascii=True, indent=2)


def load_state():
    base = {
        "inProgress": False,
        "stage": "idle",
        "lastError": "",
        "targetVersion": "",
        "startedAtUtc": None,
        "finishedAtUtc": None,
        "lastUpdateResult": "",
    }
    base.update(load_json(STATE_FILE))
    return base


def save_state(**updates):
    with _state_lock:
        state = load_state()
        state.update(updates)
        write_json(STATE_FILE, state)
        return state


def load_local_manifest():
    manifest = load_json(MANIFEST_PATH)
    current_version = str(manifest.get("currentVersion") or os.getenv("UPDATE_CURRENT_VERSION", "0.1.0"))
    current_commit = str(manifest.get("currentCommit") or os.getenv("UPDATE_CURRENT_COMMIT", ""))
    build_branch = str(manifest.get("buildBranch") or os.getenv("UPDATE_GIT_BRANCH", ""))
    channel = str(manifest.get("channel") or CHANNEL)
    deployed_at = str(manifest.get("deployedAtUtc") or "")
    target_version = str(manifest.get("targetVersion") or current_version)
    return {
        "manifest": manifest,
        "currentVersion": current_version,
        "targetVersion": target_version,
        "currentCommit": current_commit,
        "buildBranch": build_branch,
        "channel": channel,
        "deployedAtUtc": deployed_at,
    }


def fetch_remote_manifest():
    if not MANIFEST_BASE_URL:
        return None
    url = f"{MANIFEST_BASE_URL}/{ENVIRONMENT}.json"
    request = urllib.request.Request(url, headers={"Accept": "application/json"})
    with urllib.request.urlopen(request, timeout=10) as response:
        return json.loads(response.read().decode("utf-8"))


def resolve_remote_target(payload):
    if not isinstance(payload, dict):
        return None, None, None
    version = payload.get("latest_version") or payload.get("version")
    tag = payload.get("tag") or version
    commit = payload.get("commit")
    return str(version or ""), str(tag or ""), str(commit or "")


def read_env_var(key):
    path = Path(ENV_FILE)
    if not path.exists():
        return ""
    for line in path.read_text(encoding="utf-8").splitlines():
        if not line or line.strip().startswith("#") or "=" not in line:
            continue
        env_key, env_val = line.split("=", 1)
        if env_key.strip() == key:
            return env_val.strip()
    return ""


def write_env_var(key, value):
    path = Path(ENV_FILE)
    path.parent.mkdir(parents=True, exist_ok=True)
    lines = []
    found = False
    if path.exists():
        lines = path.read_text(encoding="utf-8").splitlines()
    new_lines = []
    for line in lines:
        if line.startswith(f"{key}="):
            new_lines.append(f"{key}={value}")
            found = True
        else:
            new_lines.append(line)
    if not found:
        new_lines.append(f"{key}={value}")
    path.write_text("\n".join(new_lines) + "\n", encoding="utf-8")


def run_compose(*args):
    cmd = ["docker", "compose", "--env-file", ENV_FILE, "-f", COMPOSE_FILE]
    if COMPOSE_OVERRIDE_FILE:
        cmd.extend(["-f", COMPOSE_OVERRIDE_FILE])
    cmd.extend(args)
    result = subprocess.run(
        cmd,
        cwd=PROJECT_DIR,
        capture_output=True,
        text=True,
        timeout=600,
    )
    if result.returncode != 0:
        details = result.stderr or result.stdout or "docker compose failed"
        raise RuntimeError(details)
    return result.stdout


def wait_health():
    for _ in range(HEALTH_ATTEMPTS):
        try:
            with urllib.request.urlopen(APP_HEALTH_URL, timeout=4) as response:
                if response.status == 200:
                    return True
        except Exception:
            pass
        time.sleep(HEALTH_SLEEP_SECONDS)
    return False


def set_lock():
    LOCK_FILE.write_text(now_utc_iso(), encoding="utf-8")


def clear_lock():
    if LOCK_FILE.exists():
        LOCK_FILE.unlink()


def is_locked():
    return LOCK_FILE.exists()


def update_local_manifest(current_version, target_version, commit):
    current = load_local_manifest()
    payload = current["manifest"]
    payload.update(
        {
            "service": "portal-client",
            "channel": CHANNEL,
            "currentVersion": current_version,
            "targetVersion": target_version,
            "currentCommit": commit,
            "buildBranch": ENVIRONMENT,
            "deployedAtUtc": now_utc_iso(),
        }
    )
    write_json(MANIFEST_PATH, payload)


def perform_update():
    previous_tag = read_env_var("IMAGE_TAG")
    try:
        save_state(
            inProgress=True,
            stage="fetching_manifest",
            lastError="",
            startedAtUtc=now_utc_iso(),
            finishedAtUtc=None,
            lastUpdateResult="",
        )
        remote_manifest = fetch_remote_manifest()
        remote_version, target_tag, remote_commit = resolve_remote_target(remote_manifest)
        if not target_tag:
            raise RuntimeError("Manifesto remoto sem tag/version.")

        if previous_tag == target_tag:
            save_state(
                inProgress=False,
                stage="idle",
                targetVersion=remote_version or target_tag,
                finishedAtUtc=now_utc_iso(),
                lastUpdateResult="already_current",
            )
            return

        save_state(stage="pulling_image", targetVersion=remote_version or target_tag)
        write_env_var("IMAGE_TAG", target_tag)
        write_env_var("UPDATE_TARGET_VERSION", remote_version or target_tag)
        if remote_commit:
            write_env_var("UPDATE_CURRENT_COMMIT", remote_commit)

        run_compose("pull", APP_SERVICE_NAME)
        save_state(stage="restarting_app")
        run_compose("up", "-d", "--no-build", APP_SERVICE_NAME)

        save_state(stage="validating_health")
        if not wait_health():
            raise RuntimeError("Healthcheck falhou apos update.")

        update_local_manifest(remote_version or target_tag, remote_version or target_tag, remote_commit)
        save_state(
            inProgress=False,
            stage="idle",
            finishedAtUtc=now_utc_iso(),
            lastUpdateResult="success",
            lastError="",
        )
    except Exception as exc:
        rollback_error = ""
        try:
            if previous_tag:
                write_env_var("IMAGE_TAG", previous_tag)
                run_compose("up", "-d", "--no-build", APP_SERVICE_NAME)
        except Exception as rb_exc:
            rollback_error = f" Rollback falhou: {rb_exc}"

        save_state(
            inProgress=False,
            stage="failed",
            finishedAtUtc=now_utc_iso(),
            lastError=f"{exc}{rollback_error}",
            lastUpdateResult="failed",
        )
    finally:
        clear_lock()


def start_update():
    if is_locked():
        return False, "Update ja em andamento."
    set_lock()
    thread = threading.Thread(target=perform_update, daemon=True)
    thread.start()
    return True, "Update iniciado."


class Handler(BaseHTTPRequestHandler):
    def _write_json(self, payload, status=200):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def _is_authorized(self):
        if not UPDATE_TOKEN:
            return True
        auth = self.headers.get("Authorization", "")
        return auth == f"Bearer {UPDATE_TOKEN}"

    def do_GET(self):
        if self.path == "/health":
            self._write_json({"status": "ok", "service": "update"})
            return

        if self.path == "/api/version":
            local = load_local_manifest()
            state = load_state()

            has_update = False
            target_version = local["targetVersion"]
            remote_commit = ""
            try:
                remote_manifest = fetch_remote_manifest()
                remote_version, remote_tag, remote_commit = resolve_remote_target(remote_manifest)
                if remote_version:
                    target_version = remote_version
                if remote_tag and remote_tag != read_env_var("IMAGE_TAG"):
                    has_update = True
                if remote_version and remote_version != local["currentVersion"]:
                    has_update = True
            except Exception:
                pass

            self._write_json(
                {
                    "service": "update",
                    "channel": local["channel"],
                    "currentVersion": local["currentVersion"],
                    "targetVersion": target_version,
                    "hasUpdate": has_update,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                    "deployedAtUtc": local["deployedAtUtc"],
                    "runtime": state,
                    "gitStatus": {
                        "enabled": bool(remote_commit),
                        "currentCommit": local["currentCommit"],
                        "latestCommit": remote_commit,
                        "hasUpdate": bool(remote_commit and local["currentCommit"] and remote_commit != local["currentCommit"]),
                    },
                }
            )
            return

        if self.path == "/api/update/status":
            if not self._is_authorized():
                self._write_json({"error": "unauthorized"}, status=401)
                return
            self._write_json(load_state())
            return

        self._write_json({"error": "not_found"}, status=404)

    def do_POST(self):
        if self.path in ("/api/update/start", "/api/update"):
            if not self._is_authorized():
                self._write_json({"error": "unauthorized"}, status=401)
                return
            started, message = start_update()
            self._write_json({"started": started, "message": message}, status=202 if started else 409)
            return

        self._write_json({"error": "not_found"}, status=404)

    def log_message(self, fmt, *args):
        return


if __name__ == "__main__":
    server = HTTPServer(("0.0.0.0", 8081), Handler)
    server.serve_forever()
