import json
import os
import urllib.request
import urllib.error
from http.server import BaseHTTPRequestHandler, HTTPServer
from pathlib import Path


CURRENT_VERSION = os.getenv("UPDATE_CURRENT_VERSION", "0.1.0")
TARGET_VERSION = os.getenv("UPDATE_TARGET_VERSION", CURRENT_VERSION)
CHANNEL = os.getenv("UPDATE_CHANNEL", "stable")
REQUIRES_ADMIN_APPROVAL = os.getenv("UPDATE_REQUIRES_ADMIN_APPROVAL", "true").lower() == "true"
GIT_PROVIDER = os.getenv("UPDATE_GIT_PROVIDER", "github")
GIT_REPOSITORY = os.getenv("UPDATE_GIT_REPOSITORY", "")
GIT_BRANCH = os.getenv("UPDATE_GIT_BRANCH", "main")
CURRENT_COMMIT = os.getenv("UPDATE_CURRENT_COMMIT", "")
GIT_TOKEN = os.getenv("UPDATE_GIT_TOKEN", "")
MANIFEST_PATH = os.getenv("UPDATE_MANIFEST_PATH", "/app/manifest/update-manifest.json")


def load_manifest():
    path = Path(MANIFEST_PATH)
    if not path.exists():
        return {}

    try:
        with path.open("r", encoding="utf-8") as file:
            payload = json.load(file)
            return payload if isinstance(payload, dict) else {}
    except Exception:
        return {}


def get_current_state():
    manifest = load_manifest()

    current_version = str(manifest.get("currentVersion") or CURRENT_VERSION)
    target_version = str(manifest.get("targetVersion") or TARGET_VERSION or current_version)
    channel = str(manifest.get("channel") or CHANNEL)
    current_commit = str(manifest.get("currentCommit") or CURRENT_COMMIT)
    build_branch = str(manifest.get("buildBranch") or GIT_BRANCH)
    deployed_at_utc = str(manifest.get("deployedAtUtc") or "")

    return {
        "manifest": manifest,
        "currentVersion": current_version,
        "targetVersion": target_version,
        "channel": channel,
        "currentCommit": current_commit,
        "buildBranch": build_branch,
        "deployedAtUtc": deployed_at_utc,
    }


def get_remote_commit_status(current_commit, branch):
    if GIT_PROVIDER != "github" or not GIT_REPOSITORY or not branch:
        return {
            "provider": GIT_PROVIDER,
            "repository": GIT_REPOSITORY,
            "branch": branch,
            "enabled": False,
            "message": "Git lookup is not configured."
        }

    request = urllib.request.Request(
        f"https://api.github.com/repos/{GIT_REPOSITORY}/commits/{branch}",
        headers={
            "Accept": "application/vnd.github+json",
            **({"Authorization": f"Bearer {GIT_TOKEN}"} if GIT_TOKEN else {})
        }
    )

    try:
        with urllib.request.urlopen(request, timeout=10) as response:
            payload = json.loads(response.read().decode("utf-8"))
            latest_commit = payload.get("sha", "")
            return {
                "provider": "github",
                "repository": GIT_REPOSITORY,
                "branch": branch,
                "enabled": True,
                "latestCommit": latest_commit,
                "currentCommit": current_commit,
                "hasUpdate": bool(current_commit) and latest_commit and latest_commit != current_commit,
                "message": "Git lookup completed."
            }
    except urllib.error.HTTPError as exc:
        return {
            "provider": "github",
            "repository": GIT_REPOSITORY,
            "branch": branch,
            "enabled": True,
            "message": f"Git lookup failed with HTTP {exc.code}."
        }
    except Exception as exc:
        return {
            "provider": "github",
            "repository": GIT_REPOSITORY,
            "branch": branch,
            "enabled": True,
            "message": f"Git lookup failed: {exc}"
        }


class Handler(BaseHTTPRequestHandler):
    def _write_json(self, payload, status=200):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def do_GET(self):
        if self.path == "/health":
            self._write_json({"status": "ok", "service": "update"})
            return

        if self.path == "/api/version":
            state = get_current_state()
            git_status = get_remote_commit_status(state["currentCommit"], state["buildBranch"])
            has_update_by_version = state["currentVersion"] != state["targetVersion"]
            has_update_by_git = bool(git_status.get("hasUpdate"))

            target_version = state["targetVersion"]
            if not has_update_by_version and has_update_by_git and git_status.get("latestCommit"):
                target_version = f"{state['currentVersion']}+{git_status['latestCommit'][:7]}"

            self._write_json(
                {
                    "service": "update",
                    "channel": state["channel"],
                    "currentVersion": state["currentVersion"],
                    "targetVersion": target_version,
                    "hasUpdate": has_update_by_version or has_update_by_git,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                    "deployedAtUtc": state["deployedAtUtc"],
                    "gitStatus": git_status,
                }
            )
            return

        if self.path == "/api/version/git-status":
            state = get_current_state()
            self._write_json(get_remote_commit_status(state["currentCommit"], state["buildBranch"]))
            return

        self._write_json({"error": "not_found"}, status=404)

    def do_POST(self):
        if self.path == "/api/version/check":
            state = get_current_state()
            git_status = get_remote_commit_status(state["currentCommit"], state["buildBranch"])
            self._write_json(
                {
                    "service": "update",
                    "message": "Validation completed.",
                    "currentVersion": state["currentVersion"],
                    "targetVersion": state["targetVersion"],
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                    "deployedAtUtc": state["deployedAtUtc"],
                    "gitStatus": git_status,
                }
            )
            return

        self._write_json({"error": "not_found"}, status=404)

    def log_message(self, format, *args):
        return


if __name__ == "__main__":
    server = HTTPServer(("0.0.0.0", 8081), Handler)
    server.serve_forever()
