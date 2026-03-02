import json
import os
from http.server import BaseHTTPRequestHandler, HTTPServer


CURRENT_VERSION = os.getenv("UPDATE_CURRENT_VERSION", "0.1.0")
TARGET_VERSION = os.getenv("UPDATE_TARGET_VERSION", CURRENT_VERSION)
CHANNEL = os.getenv("UPDATE_CHANNEL", "stable")
REQUIRES_ADMIN_APPROVAL = os.getenv("UPDATE_REQUIRES_ADMIN_APPROVAL", "true").lower() == "true"


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
            self._write_json(
                {
                    "service": "update",
                    "channel": CHANNEL,
                    "currentVersion": CURRENT_VERSION,
                    "targetVersion": TARGET_VERSION,
                    "hasUpdate": CURRENT_VERSION != TARGET_VERSION,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                }
            )
            return

        self._write_json({"error": "not_found"}, status=404)

    def do_POST(self):
        if self.path == "/api/version/check":
            self._write_json(
                {
                    "service": "update",
                    "message": "Validation completed.",
                    "currentVersion": CURRENT_VERSION,
                    "targetVersion": TARGET_VERSION,
                    "requiresAdminApproval": REQUIRES_ADMIN_APPROVAL,
                }
            )
            return

        self._write_json({"error": "not_found"}, status=404)

    def log_message(self, format, *args):
        return


if __name__ == "__main__":
    server = HTTPServer(("0.0.0.0", 8081), Handler)
    server.serve_forever()
