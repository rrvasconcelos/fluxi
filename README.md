# Fluxi

Fluxi é um app pessoal de controle financeiro com foco em múltiplas contas, importação de extratos e visão mensal. O repositório atual está orientado para API em .NET e ainda não inclui frontend.

## Estrutura do repositório

```text
fluxi/
├── src/
│   ├── Fluxi.Api/
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   └── appsettings*.json
│   ├── Fluxi.Application/
│   │   └── Services/
│   ├── Fluxi.Domain/
│   │   └── Entities/
│   └── Fluxi.Infrastructure/
│       ├── Data/
│       ├── Repositories/
│       └── External/
├── tests/
│   └── Fluxi.Api.Tests/
├── .github/
│   └── copilot-instructions.md
├── fluxi-schema.md
├── fluxi-dbdiagram.dbml
├── Fluxi.slnx
├── .gitignore
└── README.md
```

## Princípios

- manter o domínio claro e desacoplado
- evitar complexidade prematura
- priorizar a v1 funcional em vez da arquitetura perfeita
- separar API, aplicação, domínio e infraestrutura
- manter schema e diagrama sincronizados

## Stack atual

- Backend: .NET 10 / ASP.NET Core Web API
- Banco: PostgreSQL (a definir em momento oportuno)
- Testes: xUnit
- Frontend: ainda não será incluído neste repositório

## Próximo passo recomendado

Começar pela API com o mínimo funcional para:
- contas
- categorias
- regras de categoria
- transações
- rendas e despesas fixas
- visão mensal

## Observação

A arquitetura exata pode ser refinada depois, mas por enquanto a ideia é manter a base em .NET, com API como entrega principal e sem frontend no escopo inicial.
