# Adaptação da especificação para C#

Substitui-se o back-end PHP descrito na especificação original por uma **ASP.NET Core Web API**. O front-end continua independente e comunica por `fetch` e JSON.

| Antes | Agora |
|---|---|
| ficheiros PHP em `api/` | controllers C# em `Controllers/` |
| PDO | Entity Framework Core + provider MySQL |
| `$_SESSION` | autenticação a definir (JWT ou cookie) |
| validações em PHP | validações em modelos/DTOs e controllers C# |

O modelo de dados e os requisitos funcionais do ficheiro original mantêm-se. A escolha entre JWT e autenticação por cookie deve ser feita antes de implementar o login, pois altera a forma como o front-end envia pedidos autenticados.
