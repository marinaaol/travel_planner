# Guia de back-end — VOYAGE

## A ideia antes do código

Imagine um restaurante: o front-end é o menu e a mesa; a API é o empregado que recebe o pedido; a base de dados é a cozinha/arquivo onde a informação é guardada. O JavaScript nunca deve falar diretamente com o MySQL. Fala com a API; a API valida e guarda.

## Ordem recomendada

1. **Criar a API.** Abra o terminal na pasta `backend` e execute `dotnet new webapi -n Voyage.Api` apenas se quiser gerar novamente a base. A pasta já contém uma base mínima para estudar.
2. **Confirmar que arranca.** Em `backend/Voyage.Api`, execute `dotnet run`. Visite o endereço apresentado; `/api/health` deve devolver uma confirmação.
3. **Criar o MySQL e as tabelas.** Comece com `Usuarios`, depois `Roteiros`, finalmente `Atividades`. Cada atividade pertence a um roteiro; cada roteiro pertence a um utilizador.
4. **Ligar C# ao MySQL.** Configure a ligação fora do Git e crie o `VoyageDbContext`.
5. **Criar modelos.** Uma classe C# representa cada tabela, como uma ficha em papel representa cada tipo de informação.
6. **Implementar autenticação.** Registo, login e proteção das rotas antes do CRUD.
7. **Implementar CRUD dos roteiros.** Só depois avance para atividades.
8. **Testar a API** com Swagger ou Postman em cada passo.

## O que é CRUD?

CRUD descreve as quatro ações básicas sobre dados:

| Letra | Ação | Exemplo no VOYAGE | HTTP |
|---|---|---|---|
| C | Create / Criar | criar um roteiro | `POST /api/roteiros` |
| R | Read / Ler | listar os meus roteiros | `GET /api/roteiros` |
| U | Update / Atualizar | mudar uma data | `PUT /api/roteiros/5` |
| D | Delete / Apagar | remover um roteiro | `DELETE /api/roteiros/5` |

### Exemplo mental: criar um roteiro

1. O formulário recolhe título, destino e datas.
2. O JavaScript envia estes valores em JSON para `POST /api/roteiros`.
3. O controller C# confirma se os campos são válidos e se o utilizador está autenticado.
4. O `DbContext` guarda o registo no MySQL.
5. A API responde `201 Created`; o front-end mostra o novo cartão.

Não salte a validação: o navegador é uma porta, não um segurança. A API é que decide se o dado entra.
