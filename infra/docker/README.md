# Infraestrutura Docker

O compose ativo do repositório foi movido para `./compose`, com uma base comum e overrides por ambiente.

Consulte `docs/DEPLOY_LOCAL.md` para:

- comandos de `up`, `logs` e `down`
- arquivos `.env` por ambiente
- uso da rede externa `proxy` com Nginx Proxy Manager

Os Dockerfiles de apoio permanecem em `infra/docker`, mas o compose legado desta pasta nao e mais o ponto de entrada recomendado.
