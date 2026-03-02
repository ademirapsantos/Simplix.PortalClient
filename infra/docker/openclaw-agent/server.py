import json
import os
from http.server import BaseHTTPRequestHandler, HTTPServer


AGENT_NAME = os.getenv("OPENCLAW_AGENT_NAME", "KMKKM OpenClaw Agent")
DEFAULT_MODEL = os.getenv("OPENCLAW_DEFAULT_MODEL", "openclaw-local")
DEFAULT_QUEUE = os.getenv("OPENCLAW_DEFAULT_QUEUE", "suporte")


class Handler(BaseHTTPRequestHandler):
    def _read_body(self):
        length = int(self.headers.get("Content-Length", "0"))
        if length == 0:
            return {}
        raw = self.rfile.read(length).decode("utf-8")
        return json.loads(raw)

    def _write_json(self, payload, status=200):
        body = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def do_GET(self):
        if self.path == "/health":
            self._write_json({"status": "ok", "service": "openclaw-agent"})
            return

        if self.path == "/api/agent/info":
            self._write_json(
                {
                    "service": "openclaw-agent",
                    "name": AGENT_NAME,
                    "model": DEFAULT_MODEL,
                    "defaultQueue": DEFAULT_QUEUE,
                }
            )
            return

        self._write_json({"error": "not_found"}, status=404)

    def do_POST(self):
        if self.path == "/api/chat/respond":
            payload = self._read_body()
            message = payload.get("message", "")
            self._write_json(
                {
                    "service": "openclaw-agent",
                    "reply": f"Recebido pelo agente {AGENT_NAME}: {message}",
                    "queue": payload.get("queue", DEFAULT_QUEUE),
                    "model": DEFAULT_MODEL,
                }
            )
            return

        if self.path == "/api/tickets/triage":
            payload = self._read_body()
            subject = payload.get("subject", "")
            self._write_json(
                {
                    "service": "openclaw-agent",
                    "subject": subject,
                    "suggestedCategory": "suporte",
                    "suggestedPriority": "media",
                    "queue": DEFAULT_QUEUE,
                }
            )
            return

        self._write_json({"error": "not_found"}, status=404)

    def log_message(self, format, *args):
        return


if __name__ == "__main__":
    server = HTTPServer(("0.0.0.0", 8082), Handler)
    server.serve_forever()
