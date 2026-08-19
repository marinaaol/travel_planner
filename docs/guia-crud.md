# CRUD na prática

Use este guia depois de criar a ligação ao MySQL. Um controller é como uma caixa de correio: cada método recebe um tipo de pedido e devolve uma resposta.

## Create — POST

`POST /api/roteiros` recebe um objeto JSON. O controller valida os campos, associa o roteiro ao utilizador autenticado e usa `Add()` seguido de `SaveChangesAsync()`.

```csharp
context.Roteiros.Add(roteiro);
await context.SaveChangesAsync();
```

`Add()` prepara o novo registo; `SaveChangesAsync()` é o momento em que ele é efetivamente gravado na base de dados.

## Read — GET

`GET /api/roteiros` consulta apenas os roteiros do utilizador autenticado. Nunca devolva todos os roteiros, porque isso exporia dados de outras pessoas.

```csharp
var roteiros = await context.Roteiros
    .Where(r => r.UsuarioId == usuarioId)
    .ToListAsync();
```

## Update — PUT

`PUT /api/roteiros/{id}` começa por procurar o registo pelo `id` e pelo `usuarioId`. Se não existir, devolve `404`; se existir, altera apenas os campos permitidos e guarda.

## Delete — DELETE

`DELETE /api/roteiros/{id}` também confirma o dono. Depois usa `Remove()` e `SaveChangesAsync()`. Ao configurar a chave estrangeira com cascade, as atividades desse roteiro são apagadas junto — como tirar uma pasta e todas as folhas que estão dentro dela.
