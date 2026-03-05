#!/bin/sh
set -eu

REMOTE_HOST="${REMOTE_HOST:-}"
REMOTE_USER="${REMOTE_USER:-}"
REMOTE_PATH="${REMOTE_PATH:-/opt/simplix/portal-client}"
REMOTE_PORT="${REMOTE_PORT:-22}"
DEPLOY_ENV="${DEPLOY_ENV:-dev}"
REGISTRY="${REGISTRY:-ghcr.io}"
REGISTRY_IMAGE="${REGISTRY_IMAGE:-}"
IMAGE_TAG="${IMAGE_TAG:-}"
SOURCE_REVISION="${SOURCE_REVISION:-}"
GIT_BRANCH="${GIT_BRANCH:-}"
REGISTRY_USERNAME="${REGISTRY_USERNAME:-}"
REGISTRY_PASSWORD="${REGISTRY_PASSWORD:-}"
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

if [ -z "$REGISTRY_IMAGE" ] || [ -z "$IMAGE_TAG" ]; then
  echo "REGISTRY_IMAGE and IMAGE_TAG are required for deployment."
  exit 1
fi

tar czf - compose infra/docker/update-service | ssh -o StrictHostKeyChecking=no -p "$REMOTE_PORT" "$REMOTE_USER@$REMOTE_HOST" "
  mkdir -p '$REMOTE_PATH' &&
  tar xzf - -C '$REMOTE_PATH'
"

ssh -o StrictHostKeyChecking=no -p "$REMOTE_PORT" "$REMOTE_USER@$REMOTE_HOST" "
  mkdir -p '$REMOTE_PATH/compose' &&
  cat > '$REMOTE_PATH/compose/update-manifest.json' <<'EOF'
{
  \"service\": \"portal-client\",
  \"channel\": \"$DEPLOY_ENV\",
  \"currentVersion\": \"$IMAGE_TAG\",
  \"targetVersion\": \"$IMAGE_TAG\",
  \"currentCommit\": \"$SOURCE_REVISION\",
  \"buildBranch\": \"$GIT_BRANCH\",
  \"deployedAtUtc\": \"$(date -u +%Y-%m-%dT%H:%M:%SZ)\"
}
EOF

  cd '$REMOTE_PATH' &&
  export UPDATE_CURRENT_VERSION='$IMAGE_TAG' &&
  export UPDATE_TARGET_VERSION='$IMAGE_TAG' &&
  export UPDATE_CURRENT_COMMIT='$SOURCE_REVISION' &&
  export DEPLOY_ENV='$DEPLOY_ENV' &&
  export REGISTRY_IMAGE='$REGISTRY_IMAGE' &&
  export IMAGE_TAG='$IMAGE_TAG' &&
  if [ -n '$REGISTRY_USERNAME' ] && [ -n '$REGISTRY_PASSWORD' ]; then
    printf '%s\n' '$REGISTRY_PASSWORD' | docker login '$REGISTRY' -u '$REGISTRY_USERNAME' --password-stdin &&
  fi &&
  docker compose --env-file '$ENV_FILE' -f '$COMPOSE_FILE_BASE' -f '$COMPOSE_FILE_OVERRIDE' pull app &&
  docker compose --env-file '$ENV_FILE' -f '$COMPOSE_FILE_BASE' -f '$COMPOSE_FILE_OVERRIDE' up -d --no-build app postgres &&
  docker compose --env-file '$ENV_FILE' -f '$COMPOSE_FILE_BASE' -f '$COMPOSE_FILE_OVERRIDE' up -d --build update
"
