# Fluxi

Fluxi é um app pessoal de controle financeiro com foco em múltiplas contas, importação de extratos e visão mensal. O repositório atual está orientado para API em .NET e ainda não inclui frontend.

## Estrutura do repositório

```text
fluxi/
├── global.json
├── Directory.Build.props
├── Directory.Packages.props
├── src/
│   ├── Fluxi.Api/
│   │   ├── Program.cs
│   │   └── appsettings*.json
│   ├── Fluxi.Application/
│   ├── Fluxi.Domain/
│   └── Fluxi.Infrastructure/
├── tests/
│   ├── Fluxi.Domain.Tests/
│   ├── Fluxi.Application.Tests/
│   ├── Fluxi.Infrastructure.Tests/
│   └── Fluxi.Api.Tests/
├── .github/
│   ├── copilot-instructions.md
│   └── fluxi-business-context.md
├── docs/
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

## Stack definida para a v1

- Backend: .NET 10 / ASP.NET Core Web API
- API: ASP.NET Core Minimal APIs
- Persistência: Entity Framework Core 10 com Npgsql
- Banco: PostgreSQL
- Contratos: OpenAPI e DTOs
- Testes: xUnit
- Frontend: ainda fora do primeiro incremento

A fundação estrutural da stack foi configurada. A implementação das regras
de negócio e das funcionalidades ainda não começou.

## Próximo passo recomendado

Próximo passo: definir a primeira feature vertical e implementar o mínimo
funcional para:
- contas
- categorias
- regras de categoria
- transações
- rendas e despesas fixas
- visão mensal

## Observação

A arquitetura exata pode ser refinada depois, mas por enquanto a ideia é manter a base em .NET, com API como entrega principal e sem frontend no escopo inicial.
