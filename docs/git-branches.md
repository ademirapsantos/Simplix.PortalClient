# Estrategia de Branches

Mapa de ambientes do projeto:

- `main` = `PRD`
- `release` = `HMG`
- `dev` = `DEV`

## Fluxo base

1. Todo desenvolvimento entra em `dev`.
2. Quando uma entrega estiver pronta para homologacao, `dev` e promovida para `release`.
3. Quando a homologacao for aprovada, `release` e promovida para `main`.

## Sugestao de branches temporarias

- `feature/<nome>` saindo de `dev`
- `bugfix/<nome>` saindo de `dev`
- `hotfix/<nome>` saindo de `main` quando necessario

## Regras recomendadas no remoto

- Bloquear push direto em `main`
- Bloquear push direto em `release`
- Exigir pull request para `release` e `main`
- Exigir build verde antes de merge
- Exigir ao menos uma aprovacao para `release` e `main`

## Observacao

Protecao de branch, politica de PR e validacoes automaticas precisam ser configuradas no provedor remoto, como GitHub, GitLab ou Azure DevOps.
