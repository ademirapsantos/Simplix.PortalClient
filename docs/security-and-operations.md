# Seguranca e operacao

## Credenciais iniciais

As contas de seed continuam sendo criadas pelo `PortalClientDbContextInitializer`, mas as senhas nao ficam mais no repositorio. Configure cada uma delas fora do codigo:

- `SeedUsers__Admin__Password`
- `SeedUsers__Sales__Password`
- `SeedUsers__Support__Password`
- `SeedUsers__Client__Password`

Para desenvolvimento local, prefira `dotnet user-secrets` no projeto Web:

```bash
dotnet user-secrets --project src/KMKKM.PortalClient.Web set "SeedUsers:Admin:Password" "Troque-Esta-Senha-Admin!"
dotnet user-secrets --project src/KMKKM.PortalClient.Web set "SeedUsers:Sales:Password" "Troque-Esta-Senha-Vendas!"
dotnet user-secrets --project src/KMKKM.PortalClient.Web set "SeedUsers:Support:Password" "Troque-Esta-Senha-Suporte!"
dotnet user-secrets --project src/KMKKM.PortalClient.Web set "SeedUsers:Client:Password" "Troque-Esta-Senha-Cliente!"
```

Se um usuario de seed estiver configurado com e-mail mas sem senha, a inicializacao falha de proposito para evitar bootstrap inseguro.

## Endurecimento aplicado

- Lockout apos 5 tentativas invalidas por 15 minutos.
- Senha nova com minimo de 12 caracteres, maiuscula, minuscula, numero, simbolo e 4 caracteres unicos.
- Cookie de autenticacao `HttpOnly`, `Secure`, `SameSite=Lax`, expiracao de 8 horas e renovacao deslizante.
- Validacao mais frequente de `security stamp`.
- Suporte a `X-Forwarded-*` para operar corretamente atras de proxy reverso.

## Fluxo de senha

- `GET/POST /Account/ForgotPassword`: solicita link temporario.
- `GET/POST /Account/ResetPassword`: aplica token de redefinicao.
- `GET/POST /Account/ChangePassword`: troca autenticada.

Configuracao do reset:

- `PasswordReset__Enabled=true|false`
- `PasswordReset__PublicOrigin=https://portal.seudominio`
- `PasswordReset__ExposeResetLinkInResponse=true|false`

Use `ExposeResetLinkInResponse=true` apenas em desenvolvimento. Em producao, entregue o link por um canal controlado.

Enquanto nao houver integracao com e-mail, o link de reset tambem e registrado no log da aplicacao para operacao manual.

## Auditoria

Os eventos de autenticacao e senha sao registrados em log estruturado com:

- tipo de evento
- sucesso ou falha
- assunto/e-mail
- `userId` quando conhecido
- IP de origem
- user-agent
- trace identifier

Isso cobre operacao imediata sem introduzir tabela nova nem migracao de banco neste pacote inicial.
