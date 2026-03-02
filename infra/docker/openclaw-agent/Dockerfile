FROM python:3.12-alpine
WORKDIR /app

COPY infra/docker/openclaw-agent/server.py ./server.py

EXPOSE 8082

CMD ["python", "server.py"]
