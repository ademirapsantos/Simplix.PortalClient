FROM python:3.12-alpine
WORKDIR /app

COPY infra/docker/update-service/server.py ./server.py

EXPOSE 8081

CMD ["python", "server.py"]
