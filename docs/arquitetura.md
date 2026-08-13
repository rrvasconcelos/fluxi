# Fluxi — Arquitetura e decisões de stack

## Visão geral

O Fluxi será um sistema de controle financeiro pessoal com arquitetura modular, seguindo Clean Architecture e mantendo a simplicidade como princípio central.

A decisão atual é tratar o projeto como um monólito bem estruturado, e não como um conjunto de microsserviços. A lógica de negócio ficará centralizada no backend, enquanto a web e o mobile serão apenas camadas de apresentação que consomem a API.

A arquitetura foi pensada para evoluir gradualmente, sem introduzir complexidade prematura.

---

## Decisão arquitetural principal

### Monólito modular
Usaremos um monólito, mas com organização modular e bem definida.

Isso significa:
- uma única aplicação de backend
- um único banco de dados PostgreSQL
- módulos bem delimitados por contexto
- separação clara entre domínio, aplicação, infraestrutura e API
- capacidade de crescer sem transformar o projeto em uma máquina de microsserviços

### Por que monólito agora?
Porque o projeto ainda está em fase inicial e o foco é resolver o problema real com menos atrito possível.

Microserviços trazem custo operacional alto e complexidade desnecessária enquanto ainda não há:
- volume real de usuários
- necessidade de isolamento de contexto
- times múltiplos e independentes
- necessidade de deploy e escalabilidade separada

---

## Stack definida para a v1

Esta é a stack definida para a v1. A fundação estrutural já foi configurada,
mas a implementação das regras de negócio e das funcionalidades ainda não
começou.

### Backend
- .NET 10
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core 10
- Npgsql para integração com PostgreSQL

### Banco de dados
- PostgreSQL único

O banco será único porque o domínio exige consistência entre contas,
transações, categorias, faturas, rendas e despesas fixas. Não haverá múltiplos
bancos na v1.

### Contratos e documentação da API
- OpenAPI
- DTOs separados das entidades de domínio
- ProblemDetails para erros HTTP

### Testes
- xUnit
- Testes unitários como base
- Testes de integração apenas quando existirem fluxos reais para validar

### Observabilidade
- Logging estruturado com os recursos nativos do ASP.NET Core
- Serilog somente se surgir uma necessidade concreta de sinks ou correlação
	avançada

### Clientes
Frontend web e mobile não fazem parte do primeiro incremento. Angular,
React Native e Expo permanecem como opções futuras, a serem decididas quando
a API e o fluxo principal estiverem validados.

### Fora do escopo inicial
- autenticação, enquanto o uso permanecer local e pessoal
- microserviços
- múltiplos bancos
- Kafka, RabbitMQ e outras plataformas de mensageria distribuída
- Redis
- parser de PDF
- Open Finance
- simulação de compra e parcelamento avançado

Autenticação e proteção de segredos serão obrigatórias antes de qualquer
exposição externa da API.

---

## Clean Architecture + Vertical Slices

A arquitetura do Fluxi será baseada em Clean Architecture, com uma abordagem híbrida que também incorpora vertical slices quando fizer sentido.

### Camadas principais
- Domain
- Application
- Infrastructure
- API

### Vertical slices
Alguns contextos serão organizados em slices por funcionalidade, por exemplo:
- Accounts
- Categories
- Transactions
- Invoices
- Dashboard
- Imports

Isso ajuda a manter o código coeso e evitar que o projeto vire uma grande massa de serviços e abstrações genéricas sem propósito.

A combinação de clean architecture + vertical slices entrega:
- clareza de responsabilidade
- baixo acoplamento
- evolução mais simples
- facilidade para manter o domínio financeiro organizado

---

## Regra de governança de negócio

O arquivo mestre de regras de negócio está em [docs/regras-negocio.md](regras-negocio.md).

Toda alteração nesse documento deve ser revisada junto com os fluxos existentes em:
- [docs/flows/fluxo-importacao.md](flows/fluxo-importacao.md)
- [docs/flows/fluxo-usuario.md](flows/fluxo-usuario.md)
- [docs/flows/fluxo-mensal.md](flows/fluxo-mensal.md)
- [docs/flows/fluxo-categoria.md](flows/fluxo-categoria.md)

Se a regra mudar, o fluxo visual também precisa mudar para refletir a nova realidade.

Esse documento e os fluxos devem permanecer sincronizados.

---

## Filosofia da arquitetura

### 1) Backend como fonte da verdade
O backend será responsável por:
- persistência
- regras de negócio
- validações
- cálculos financeiros
- autenticação e autorização
- importação e categorização de transações
- visão mensal e agregações

A web e o mobile não devem conter a lógica principal do domínio.

### 2) UI separada da lógica
Web e mobile podem ter a mesma intenção funcional, mas devem ter experiências visuais diferentes.

Exemplo:
- web: dashboards, listas densas, tabelas e relatórios
- mobile: fluxo rápido, ações curtas, cards e foco em uso diário

Mesmo com telas distintas, a regra de negócio continua única no backend.

### 3) Reuso útil, não reaproveitamento forçado
O projeto deve reaproveitar:
- contratos da API
- regras de negócio do backend
- domínio e entidades
- modelos de dados

O projeto não deve tentar reaproveitar 1:1:
- layout visual
- componentes de interface
- UX específica de plataforma
- estrutura de navegação e gerenciamento de tela

---

## Banco de dados

### Decisão atual
Usaremos um único banco PostgreSQL para todo o sistema.

### Por que um único banco?
Porque o domínio financeiro exige consistência e correlação entre informações como:
- contas
- transações
- categorias
- faturas
- receitas
- despesas fixas

Esse conjunto precisa ser consultado de forma integrada para gerar visão mensal e relatórios.

### O que isso implica
- uma base coerente
- transações ACID
- queries mais simples
- menos complexidade operacional
- maior consistência para dados financeiros

### Quando evoluir para múltiplos bancos?
Só quando realmente existir necessidade de isolamento forte por domínio ou carga, e não por hipótese.

---

## Mensageria e processamento assíncrono

### Decisão atual
Não usaremos mensageria complexa agora.

### Quando usaremos async
Apenas quando houver necessidade real de processamento em background, por exemplo:
- importação de arquivos grandes
- parsing de PDF
- processamento em lote
- relatórios pesados
- tarefas de notificação

### Como faremos isso no início
- processos assíncronos simples dentro da aplicação
- hosted services / workers leves
- filas internas ou job runner simples quando necessário

### O que não faremos agora
- Kafka
- RabbitMQ
- arquiteturas distribuídas de eventos com complexidade de coordenação

A ideia é manter o sistema simples e previsível enquanto ainda valida a utilidade real do produto.

---

## Fluxos síncronos e assíncronos

### Síncronos
Usaremos comunicação síncrona para:
- cadastro de conta
- criação de transação manual
- categorização direta
- consultas de saldo
- cálculo de visão mensal
- manutenção de categorias e regras

### Assíncronos
Usaremos fluxo assíncrono para:
- importação de arquivos
- processamento pesado
- geração de relatórios complexos
- jobs de manutenção ou tarefas em lote

### Regra geral
As operações que afetam o dado financeiro e exigem consistência devem ser síncronas e transacionais.

Operações mais pesadas, de processamento posterior, podem ser assíncronas.

---

## Por que não MAUI como escolha principal

A ideia de usar MAUI é interessante, mas por enquanto não é uma decisão para
o primeiro incremento deste projeto.

Principais motivos:
- o projeto ainda está em fase de definição da arquitetura e do produto
- o foco atual é aprender e construir um produto útil, não travar em uma stack mobile específica
- a escolha do cliente mobile será feita somente depois da validação do
	backend e do fluxo principal
- MAUI pode ser reavaliado no futuro, mas não está definido como parte da v1

---

## Decisão de produto e arquitetura

### 1) Web e mobile serão produtos distintos, não clones perfeitos
A web será mais analítica e de consulta.
O mobile será mais rápido e direto, com foco em uso diário.

### 2) O backend será o centro do sistema
Tudo que for regra de negócio, cálculo ou persistência deve viver no backend.

### 3) Duplicação aceitável
É aceitável duplicar:
- componentes de interface
- navegação
- layouts visuais
- interação de usuário por plataforma

Não é aceitável duplicar:
- regras financeiras
- regras de categoria
- processamento de transações
- lógica de saldo
- lógica de fatura
- regras de importação

---

## Estrutura de projeto sugerida

O projeto deve ser organizado em múltiplos projetos, mantendo a API como principal entrega inicial.

### Estrutura sugerida

```text
fluxi/
├── src/
│   ├── Fluxi.Api/
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   └── appsettings*.json
│   ├── Fluxi.Application/
│   │   ├── Features/
│   │   ├── Services/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Fluxi.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Aggregates/
│   ├── Fluxi.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── Integrations/
│   └── Fluxi.Shared/
│       └── Contracts/
├── tests/
│   └── Fluxi.Api.Tests/
├── apps/
│   ├── web-angular/
│   └── mobile-react-native/
├── docs/
│   ├── fluxi-schema.md
│   ├── fluxi-dbdiagram.dbml
│   └── arquitetura.md
├── Fluxi.sln
├── README.md
└── .gitignore
```

---

## Regra de governança para o projeto

### Backend
- a lógica principal vive aqui
- tudo que impacta domínio e persistência passa pela API

### Web
- consumir a API
- apresentar visualização e operações de gestão
- não reinventar regras de negócio

### Mobile
- consumir a API
- priorizar UX de uso diário
- manter foco em velocidade e simplicidade

---

## Decisão final

A arquitetura recomendada para o Fluxi é:

- monólito modular
- Clean Architecture
- abordagem híbrida com vertical slices
- PostgreSQL como banco único
- .NET API no backend
- Angular na web
- React Native + Expo no mobile
- sem microsserviços e sem mensageria complexa no início

Essa combinação é coerente com:
- o tamanho real do projeto
- a necessidade de velocidade e simplicidade
- a importância de consistência financeira
- o objetivo de evoluir sem criar complexidade desnecessária

---

## Observação de evolução

Essa arquitetura é a recomendação inicial, mas não é fixa para sempre. O sistema pode evoluir conforme a necessidade real do produto.

O importante é manter a base simples, coerente e madura, e não criar complexidade antes da necessidade real.
