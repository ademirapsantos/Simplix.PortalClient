import json
import os
import urllib.request
import urllib.error
from http.server import BaseHTTPRequestHandler, HTTPServer


CURRENT_VERSION = os.getenv("UPDATE_CURRENT_VERSION", "0.1.0")
TARGET_VERSION = os.getenv("UPDATE_TARGET_VERSION", CURRENT_VERSION)
CHANNEL = os.getenv("UPDATE_CHANNEL", "stable")
REQUIRES_ADMIN_APPROVAL = os.getenv("UPDATE_REQUIRES_ADMIN_APPROVAL", "true").lower() == "true"
GIT_PROVIDER = os.getenv("UPDATE_GIT_PROVIDER", "github")
GIT_REPOSITORY = os.getenv("UPDATE_GIT_REPOSITORY", "")
GIT_BRANCH = os.getenv("UPDATE_GIT_BRANCH", "main")
CURRENT_COMMIT = os.getenv("UPDATE_CURRENT_COMMIT", "")
GIT_TOKEN = os.getenv("UPDATE_GIT_TOKEN", "")


def get_remote_commit_status():
    if GIT_PROVIDER != "github" or not GIT_REPOSITORY:
        return {
            "provider": GIT_PROVIDER,
            "repository": GIT_REPOSITORY,
            "branch": GIT_BRANCH,
            "enabled": False,
            "message": "Git lookup is not configured."
        }

    request = urllib.request.Request(
        f"https://api.github.com/repos/{GIT_REPOSITORY}/commits/{GIT_BRANCH}",
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
                "branch": GIT_BRANCH,
                "enabled": True,
                "latestCommit": latest_commit,
                "currentCommit": CURRENT_COMMIT,
                "hasUpdate": bool(CURRENT_COMMIT) and latest_commit and latest_commit != CURRENT_COMMIT,
                "message": "Git lookup completed."
            }
    except urllib.error.HTTPError as exc:
        return {
            "provider": "github",
            "repository": GIT_REPOSITORY,
            "branch": GIT_BRANCH,
            "enabled": True,
            "message": f"Git lookup failed with HTTP {exc.code}."
        }
    except Exception as exc:
        return {
            "provider": "github",
            "repository": GIT_REPOSITORY,
            "branch": GIT_BRANCH,
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
            git_status = get_remote_commit_status()
            self._write_json(
                {
                    "service": "update",
                    "channel": CHANNEL,
                    "currentVersion": CURRENT_VERSION,
                    "targetVersion": TARGET_VERSION,
                    "hasUpdate": CURRENT_VERSION != TARGET_VERSION,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                    "gitStatus": git_status,
                }
            )
            return

        if self.path == "/api/version/git-status":
            self._write_json(get_remote_commit_status())
            return

        self._write_json({"error": "not_found"}, status=404)

    def do_POST(self):
        if self.path == "/api/version/check":
            git_status = get_remote_commit_status()
            self._write_json(
                {
                    "service": "update",
                    "message": "Validation completed.",
                    "currentVersion": CURRENT_VERSION,
                    "targetVersion": TARGET_VERSION,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
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
