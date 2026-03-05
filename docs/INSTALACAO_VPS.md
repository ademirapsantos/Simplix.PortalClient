# Instalacao da aplicacao na VPS (Docker Compose)

Este guia cobre somente a instalacao manual da aplicacao na VPS, partindo do repositorio Git ate a aplicacao em execucao.

Premissas:

- Linux ja configurado
- Docker Engine + Docker Compose plugin ja instalados
- Proxy reverso ja instalado (rede Docker `proxy`)

Estrutura padrao da VPS:

```text
/srv/
  apps/
    simplix/
      portal-client
  Infra/
    actions-runner/
    nginx-proxy-manager/
```

## 1. Acessar a VPS e preparar pasta da aplicacao

```bash
ssh seu_usuario@seu_host
sudo mkdir -p /srv/apps/simplix/portal-client
sudo chown -R $USER:$USER /srv/apps/simplix/portal-client
cd /srv/apps/simplix/portal-client
```

## 2. Clonar o projeto

```bash
git clone https://github.com/SEU_ORG/Simplix.PortalClient.git .
```

Se o repositorio for privado, use PAT:

```bash
git clone https://<GITHUB_USER>:<GITHUB_TOKEN>@github.com/SEU_ORG/Simplix.PortalClient.git .
```

## 3. Escolher o ambiente e branch

Mapeamento atual:

- `dev` -> ambiente `DEV`
- `release` -> ambiente `HMG`
- `main` -> ambiente `PRD`

Exemplo para producao:

```bash
git checkout main
```

## 4. Garantir que a rede externa do proxy existe

```bash
docker network ls | grep proxy || docker network create proxy
```

## 5. Configurar arquivo `.env` do ambiente

Arquivos disponiveis:

- `compose/.env.dev`
- `compose/.env.hmg`
- `compose/.env.prd`
- `compose/.env.example`

Exemplo (PRD):

```bash
cp compose/.env.prd compose/.env.prd.local
nano compose/.env.prd.local
```

Campos minimos para revisar/ajustar:

- `POSTGRES_PASSWORD`
- `UPDATE_TOKEN`
- `UPDATE_GIT_REPOSITORY`
- `UPDATE_GIT_BRANCH`
- `UPDATE_GIT_TOKEN` (se repositorio privado)
- `UPDATE_MANIFEST_BASE_URL`
- `DB_INIT_*` (em PRD, recomendado manter `false`)

## 6. Subir os containers

Use os arquivos do ambiente escolhido.

### DEV

```bash
docker compose --env-file compose/.env.dev -f compose/docker-compose.yml -f compose/docker-compose.dev.yml up -d --build
```

### HMG

```bash
docker compose --env-file compose/.env.hmg -f compose/docker-compose.yml -f compose/docker-compose.hmg.yml up -d --build
```

### PRD (usando o `.env` local do exemplo acima)

```bash
docker compose --env-file compose/.env.prd.local -f compose/docker-compose.yml -f compose/docker-compose.prd.yml up -d --build
```

## 7. Validar se ficou em execucao

```bash
docker compose --env-file compose/.env.prd.local -f compose/docker-compose.yml -f compose/docker-compose.prd.yml ps
```

Ver logs:

```bash
docker compose --env-file compose/.env.prd.local -f compose/docker-compose.yml -f compose/docker-compose.prd.yml logs -f app
```

Teste de saude local da app:

```bash
docker exec simplix-app-prd wget -qO- http://127.0.0.1:8080/health
```

## 8. Configurar o proxy reverso

No seu proxy, direcione o dominio para:

- container: `simplix-app-prd` (ou nome do app do ambiente)
- porta interna: `8080`
- rede Docker: `proxy`

## 9. Comandos de operacao diaria

Atualizar codigo e recriar:

```bash
cd /srv/apps/simplix/portal-client
git pull origin main
docker compose --env-file compose/.env.prd.local -f compose/docker-compose.yml -f compose/docker-compose.prd.yml up -d --build
```

Parar ambiente:

```bash
docker compose --env-file compose/.env.prd.local -f compose/docker-compose.yml -f compose/docker-compose.prd.yml down
```

## Observacao sobre imagens GHCR

O `docker-compose.yml` usa por padrao:

`ghcr.io/ademirapsantos/simplix.portalclient/portal-client-app:${IMAGE_TAG}`

Se for usar imagem publicada no GitHub Container Registry em vez de build local, ajuste:

- `REGISTRY_IMAGE`
- `IMAGE_TAG`

e execute `docker login ghcr.io` antes do `up`.
