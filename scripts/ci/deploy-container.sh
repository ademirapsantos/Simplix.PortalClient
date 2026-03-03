#!/bin/sh
set -eu

REMOTE_HOST="${REMOTE_HOST:-}"
REMOTE_USER="${REMOTE_USER:-}"
REMOTE_PATH="${REMOTE_PATH:-/opt/kmkkm/portal-client}"
DEPLOY_ENV="${DEPLOY_ENV:-dev}"
COMPOSE_FILE_BASE="compose/docker-compose.yml"

case "$DEPLOY_ENV" in
  dev)
    COMPOSE_FILE_OVERRIDE="compose/docker-compose.dev.yml"
    ENV_FILE="compose/.env.dev"
    ;;
  hmg|release)
    COMPOSE_FILE_OVERRIDE="compose/docker-compose.hmg.yml"
    ENV_FILE="compose/.env.hmg"
    ;;
  prd|main)
    COMPOSE_FILE_OVERRIDE="compose/docker-compose.prd.yml"
    ENV_FILE="compose/.env.prd"
    ;;
  *)
    echo "Unsupported DEPLOY_ENV: $DEPLOY_ENV"
    exit 1
    ;;
esac

if [ -z "$REMOTE_HOST" ] || [ -z "$REMOTE_USER" ]; then
  echo "REMOTE_HOST and REMOTE_USER are required for deployment."
  exit 1
fi

ssh -o StrictHostKeyChecking=no "$REMOTE_USER@$REMOTE_HOST" "
  mkdir -p '$REMOTE_PATH' &&
  cd '$REMOTE_PATH' &&
  export DEPLOY_ENV='$DEPLOY_ENV' &&
  docker compose --env-file '$ENV_FILE' -f '$COMPOSE_FILE_BASE' -f '$COMPOSE_FILE_OVERRIDE' up -d --build
"
