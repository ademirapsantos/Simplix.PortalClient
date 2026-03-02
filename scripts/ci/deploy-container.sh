#!/bin/sh
set -eu

REMOTE_HOST="${REMOTE_HOST:-}"
REMOTE_USER="${REMOTE_USER:-}"
REMOTE_PATH="${REMOTE_PATH:-/opt/kmkkm/portal-client}"
DEPLOY_ENV="${DEPLOY_ENV:-dev}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
REGISTRY_IMAGE="${REGISTRY_IMAGE:-ghcr.io/example/kmkkm-portal-client}"

if [ -z "$REMOTE_HOST" ] || [ -z "$REMOTE_USER" ]; then
  echo "REMOTE_HOST and REMOTE_USER are required for deployment."
  exit 1
fi

ssh -o StrictHostKeyChecking=no "$REMOTE_USER@$REMOTE_HOST" "
  mkdir -p '$REMOTE_PATH' &&
  cd '$REMOTE_PATH' &&
  export IMAGE_TAG='$IMAGE_TAG' &&
  export DEPLOY_ENV='$DEPLOY_ENV' &&
  export REGISTRY_IMAGE='$REGISTRY_IMAGE' &&
  docker compose pull &&
  docker compose up -d
"
