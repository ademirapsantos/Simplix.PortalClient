# Deploy e Atualizacao

## Ambientes

- `dev` publica em `DEV`
- `release` publica em `HMG`
- `main` publica em `PRD`

## Versionamento automatico

Cada push gera uma versao automatica:

- `dev`: `1.0.<run>-dev.<sha>`
- `release`: `1.0.<run>-rc.<sha>`
- `main`: `1.0.<run>`

O pipeline injeta essa versao no build .NET e na tag da imagem Docker.

## Deploy automatico

O workflow em `.github/workflows/portal-client-cicd.yml` executa:

1. calcula a versao do commit
2. compila a solution
3. publica a imagem no registry
4. faz deploy no ambiente correspondente a branch

## Atualizacao do sistema

O servico `update` foi preparado para consultar o GitHub e comparar:

- branch alvo de publicacao
- commit atual do ambiente implantado
- ultimo commit do repositorio remoto

Se houver diferenca, o endpoint retorna `hasUpdate = true`.

## Regra de negocio prevista

Somente o usuario com perfil `Admin` deve receber a solicitacao para atualizar o sistema.
Os demais usuarios nao devem ver prompt de atualizacao.

## Secrets esperados no GitHub

- `DEPLOY_HOST`
- `DEPLOY_USER`
- `DEPLOY_PATH`
- `DEPLOY_SSH_KEY`

## Ajustes necessarios antes de producao

- trocar `your-org/simplix_PortalClient` pelo repositorio real
- definir os environments `dev`, `hmg` e `prd` no GitHub
- configurar secrets por ambiente
- substituir o deploy shell basico pelo procedimento definitivo da VPS
