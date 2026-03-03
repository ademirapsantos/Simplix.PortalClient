# Deploy local e VPS com Docker Compose

O padrao de deploy foi reorganizado para operar por branch e ambiente:

- `dev` => `DEV`
- `release` => `HMG`
- `main` => `PRD`

O `app` entra na rede interna do projeto e tambem na rede Docker externa `proxy`, onde o Nginx Proxy Manager publica o acesso HTTP. `postgres` e `update` ficam somente na rede interna e nao publicam portas no host.

## Pre-requisitos

- Docker e Docker Compose Plugin instalados
- rede Docker externa `proxy` criada na VPS:

```bash
docker network create proxy
```

## Subir DEV

```bash
docker compose --env-file compose/.env.dev -f compose/docker-compose.yml -f compose/docker-compose.dev.yml up -d --build
```

## Subir HMG

```bash
docker compose --env-file compose/.env.hmg -f compose/docker-compose.yml -f compose/docker-compose.hmg.yml up -d --build
```

## Subir PRD

```bash
docker compose --env-file compose/.env.prd -f compose/docker-compose.yml -f compose/docker-compose.prd.yml up -d --build
```

## Logs DEV

```bash
docker compose --env-file compose/.env.dev -f compose/docker-compose.yml -f compose/docker-compose.dev.yml logs -f
```

## Logs HMG

```bash
docker compose --env-file compose/.env.hmg -f compose/docker-compose.yml -f compose/docker-compose.hmg.yml logs -f
```

## Logs PRD

```bash
docker compose --env-file compose/.env.prd -f compose/docker-compose.yml -f compose/docker-compose.prd.yml logs -f
```

## Down DEV

```bash
docker compose --env-file compose/.env.dev -f compose/docker-compose.yml -f compose/docker-compose.dev.yml down
```

## Down HMG

```bash
docker compose --env-file compose/.env.hmg -f compose/docker-compose.yml -f compose/docker-compose.hmg.yml down
```

## Down PRD

```bash
docker compose --env-file compose/.env.prd -f compose/docker-compose.yml -f compose/docker-compose.prd.yml down
```

## Observacoes

- Preencha `POSTGRES_PASSWORD`, `UPDATE_GIT_TOKEN` e as senhas de seed `SEED_ADMIN_PASSWORD`, `SEED_SALES_PASSWORD`, `SEED_SUPPORT_PASSWORD`, `SEED_CLIENT_PASSWORD` antes de subir cada ambiente.
- Ajuste `UPDATE_GIT_REPOSITORY` para o repositorio real.
- Ajuste `PORTAL_PUBLIC_ORIGIN` para a URL publica do portal. O fluxo de reset de senha usa esse valor para gerar links corretos atras do proxy.
- O Nginx Proxy Manager deve apontar para o container `app` na rede `proxy`, usando a porta interna `8080`.
- Os nomes dos containers e do volume do PostgreSQL variam por ambiente para evitar conflito.
