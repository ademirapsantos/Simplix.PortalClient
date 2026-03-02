# Controle de Acesso do Repositorio

## Travamento local ja aplicado

Este repositorio possui hooks versionados em `.githooks/` para bloquear:

- `commit`
- `merge commit`
- `push`

Se a identidade Git local nao for:

- `Ademir Santos`
- `ademir574@gmail.com`

as operacoes acima falham.

## Limite importante

Isso protege a maquina e este clone local, mas nao garante controle absoluto no servidor remoto.
Controle real de:

- `pull request`
- `merge`
- permissao de escrita

so existe no GitHub, GitLab ou Azure DevOps.

## Configuracao recomendada no GitHub

1. repositorio privado
2. somente seu usuario com permissao `Admin`
3. nenhum outro colaborador com permissao de `Write`, `Maintain` ou `Admin`
4. branch protection em `dev`, `release` e `main`
5. desabilitar merge direto sem pull request em `release` e `main`
6. opcionalmente desabilitar force push e branch deletion

## Recomendacao adicional

Quando subir para o GitHub, crie tambem um arquivo `CODEOWNERS` apontando apenas para o seu usuario GitHub.
Isso nao bloqueia acesso sozinho, mas reforca revisao obrigatoria quando combinado com branch protection.
