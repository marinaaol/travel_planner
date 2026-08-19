# Guia de front-end — layout aprovado

O objetivo é transformar o protótipo visual em páginas reais, uma camada de cada vez. Não tente fazer tudo ao mesmo tempo.

1. **Mapa das páginas:** crie `index.html`, `login.html`, `registo.html`, `dashboard.html` e `roteiro.html`. Primeiro use HTML semântico: `header`, `nav`, `main`, `section`, `form` e `footer`.
2. **Variáveis visuais:** passe as cores e fontes aprovadas para `css/variables.css`. Pense nelas como uma caixa de tintas: se o vermelho mudar, altera-se uma vez.
3. **Estilos partilhados:** construa botões, cartões, inputs e cabeçalho em `css/components.css` antes de estilizar páginas isoladas.
4. **Dashboard estático:** crie um cartão de roteiro usando dados escritos no HTML. Só quando o aspeto estiver certo o transforme em JavaScript.
5. **Formulários:** adicione `label`, `id`, `required` e mensagens de erro. Um `label` é a indicação que explica ao utilizador — e ao leitor de ecrã — o que cada campo pede.
6. **JavaScript por página:** `dashboard.js` chama `GET /api/roteiros`; `roteiro.js` chama as rotas das atividades. Um ficheiro deve ter uma responsabilidade clara.
7. **Responsividade:** comece pelo telemóvel (360 px), depois tablet (768 px) e desktop (1280 px).
8. **Integração e testes:** ligue uma ação de cada vez: listar, criar, editar, apagar. Abra as ferramentas do navegador e confirme pedidos, respostas e erros.

Exemplo de pedido que irá escrever quando a API existir:

```js
const resposta = await fetch('https://localhost:porta/api/roteiros');
const roteiros = await resposta.json();
```

`fetch` é como enviar uma mensagem à receção (API) e `json()` é abrir a resposta que ela devolveu.
