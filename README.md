# VOYAGE

Planeador interativo de roteiros de viagem desenvolvido por **Marina Oliveira** como projeto académico.

## Objetivo do MVP

Permitir que um utilizador crie uma conta, elabore roteiros e organize atividades por cada dia da viagem. O foco inicial é fazer o essencial bem: autenticação de login, estruturar roteiros e aplicar atividades como check-list.

## Tecnologias

- Front-end: HTML, CSS e JavaScript (vanilla);
- Back-end: C# com ASP.NET Core Web API (.NET 10);
- Dados: MySQL;
- Acesso a dados: Entity Framework Core.

## Estrutura

```text
VOYAGE/
├── index.html                    # Layout aprovado (landing page)
├── planner.html                  # Protótipo de navegação
├── plano-implementacao.html      # Checklist interativa de aprendizagem
├── backend/Voyage.Api/           # API C# inicial
└── docs/                         # Guias de estudo e implementação
```

## Primeiro passo

1. Abra `plano-implementacao.html` no navegador.
2. Leia `docs/guia-backend.md` antes de iniciar a API.
3. Execute apenas uma tarefa de cada vez e marque-a quando a compreender e testar.

> A checklist acompanha o processo, mas não substitui os testes. Só marque uma tarefa depois de a testar.

## Funcionalidades futuras (fora do MVP)

Mapa, partilha pública de roteiros, exportação PDF, imagens de capa, filtros, modo escuro, gráficos de orçamento e reordenação por arrastar-e-largar serão desenvolvidos após o MVP estar concluído e testado.

## Licença

Este projeto está sob a licença [MIT](LICENSE).
