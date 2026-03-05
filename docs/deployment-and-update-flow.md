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

O servico `update` utiliza um manifesto de deploy (`compose/update-manifest.json`) atualizado a cada publicacao.
Esse manifesto contem:

- versao publicada
- commit publicado
- canal/branch da publicacao
- data/hora do deploy

Com base nele, o servico consulta o GitHub e compara:

- branch alvo de publicacao
- commit atual do ambiente implantado
- ultimo commit do repositorio remoto

Se houver diferenca, o endpoint retorna `hasUpdate = true`.

## Inicializacao de banco

A inicializacao automatica do banco no startup ficou controlada por configuracao:

- `DatabaseInitialization:Enabled`
- `DatabaseInitialization:ApplySchemaChanges`
- `DatabaseInitialization:SeedData`
- `DatabaseInitialization:SeedIdentity`

Para `HMG` e `PRD`, o recomendado e manter desabilitado para evitar alteracao automatica de base durante deploy.

## Regra de negocio prevista

Somente o usuario com perfil `Admin` deve receber a solicitacao para atualizar o sistema.
Os demais usuarios nao devem ver prompt de atualizacao.

## Secrets esperados no GitHub

- `DEPLOY_HOST`
- `DEPLOY_USER`
- `DEPLOY_PATH`
- `DEPLOY_SSH_KEY`

## Ajustes necessarios antes de producao

- trocar `your-org/Simplix.PortalClient` pelo repositorio real
- definir os environments `dev`, `hmg` e `prd` no GitHub
- configurar secrets por ambiente
- substituir o deploy shell basico pelo procedimento definitivo da VPS
