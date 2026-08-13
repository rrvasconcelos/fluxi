# Fluxi — Padrões e regras do projeto .NET

Este documento define as regras rígidas para a criação e manutenção do backend em .NET do Fluxi.

Ele deve ser usado como guia por IA, desenvolvedores e qualquer ferramenta de geração de código.

## Objetivo

Garantir que o projeto siga padrões consistentes de:
- organização de pastas
- nomes de classes, métodos, arquivos e projetos
- uso de Clean Architecture
- uso de vertical slices
- clareza de responsabilidade
- qualidade de código
- simplicidade funcional

Este documento tem precedência sobre convenções genéricas quando houver conflito com o projeto.

---

## Regras gerais

### 1) Simplicidade acima de abstrações sofisticadas
Não criar camadas, interfaces e abstrações que ainda não sejam necessárias.

Não criar "frameworks internos" para tudo.

A regra é:
- resolver o problema real
- sem arquitetura ornamental
- sem abstrações prematuras

### 2) Não duplicar regras de negócio
A regra de negócio deve viver no backend e não se repetir em clientes web ou mobile.

### 3) Nenhuma lógica financeira no front-end
Qualquer regra de domínio financeiro deve estar no backend.

### 4) Não inventar nomes arbitrários
Todos os nomes devem refletir o domínio do Fluxi e seguir convenção consistente.

### 5) O código deve ser legível primeiro
Priorizar clareza em vez de cleverness.

### 6) Todo objeto deve ser nomeado em inglês
Todos os nomes de classes, métodos, propriedades, arquivos, pastas, interfaces, enums, DTOs e variáveis devem ser em inglês.

Não misturar idiomas no mesmo projeto.

Exemplos corretos:
- Account
- CreateTransactionCommand
- MonthlySummary
- ImportTransactions
- CategoryRule

Exemplos incorretos:
- Conta
- criarTransacao
- ResumoMensal
- CategoriaRegra
- importacaoArquivo

### 7) Comentários e documentação também em inglês
Comentários, XML docs, documentação interna e nomes de exemplos devem seguir o inglês, salvo casos de documentação de negócio que exijam idioma específico do usuário.

### 8) Nomes de banco, schema e tabelas devem refletir o domínio em inglês
Mesmo que o projeto tenha documentação em português, a estrutura de código e a modelagem devem manter o inglês como padrão dominante.

---

## Arquitetura obrigatória

### Base
- Clean Architecture
- monólito modular
- vertical slices em contextos funcionais
- PostgreSQL como banco único
- ASP.NET Core Minimal APIs como camada de entrada

### Organização da solução

- projetos de produção ficam em `src/`
- projetos de teste ficam em `tests/`
- documentação fica em `docs/`
- instruções e automação do repositório ficam em `.github/`
- propriedades compartilhadas ficam em `Directory.Build.props`
- versões de pacotes NuGet ficam em `Directory.Packages.props`

Não criar pastas genéricas ou projetos vazios sem uma responsabilidade
concreta. Vertical slices devem ser introduzidos conforme features reais
forem implementadas.

### Camadas obrigatórias
1. SharedKernel
2. Domain
3. Application
4. Infrastructure
5. API

### Regras de camada
- SharedKernel não depende de nenhuma outra camada do Fluxi.
- Domain não depende de Application, Infrastructure ou API.
- Domain pode depender de SharedKernel somente quando houver um conceito
  realmente compartilhado.
- Application depende de Domain, mas não de Infrastructure.
- Application pode depender de SharedKernel somente quando houver um conceito
  realmente compartilhado.
- Infrastructure depende de Domain e Application.
- Infrastructure pode depender de SharedKernel somente quando houver um
  conceito realmente compartilhado.
- API depende de Application e Infrastructure conforme necessário.

O `SharedKernel` não deve se tornar uma pasta genérica ou um depósito de
abstrações compartilhadas por conveniência. Cada tipo adicionado deve ter uma
justificativa de compartilhamento entre contextos ou camadas.

### Vertical slices
Para contextos de funcionalidade, a organização pode ser por feature, por exemplo:
- Accounts
- Categories
- Transactions
- Invoices
- Imports
- Dashboard

A estrutura pode seguir:

```text
src/
  Fluxi.SharedKernel/
  Fluxi.Api/
  Fluxi.Application/
  Fluxi.Domain/
  Fluxi.Infrastructure/
```

Dentro do Application, priorizar organização por feature e não por camada genérica.

Os projetos de teste devem acompanhar as camadas de produção:

```text
tests/
  Fluxi.SharedKernel.Tests/
  Fluxi.Domain.Tests/
  Fluxi.Application.Tests/
  Fluxi.Infrastructure.Tests/
  Fluxi.Api.Tests/
```

Cada projeto de teste deve referenciar somente a camada necessária e não deve
duplicar testes de outra camada.

Exemplo:

```text
Fluxi.Application/
  Accounts/
    CreateAccount/
    GetAccounts/
    UpdateAccount/
  Transactions/
    CreateTransaction/
    ImportTransactions/
    GetMonthlySummary/
```

---

## Nomeclatura obrigatória

### 1) Pastas e arquivos
Usar PascalCase para nomes de pastas e arquivos quando representam tipos, features ou contextos.

Exemplos:
- Accounts
- Transactions
- CreateTransaction
- MonthlySummary
- ImportTransactions

Evitar:
- account
- transactionService
- util
- helpers
- misc

### 2) Classes
Usar PascalCase.

Exemplos:
- Account
- Transaction
- CreateTransactionCommand
- ImportTransactionsHandler

### 3) Métodos
Usar PascalCase para métodos públicos e camelCase para parâmetros locais.

Exemplos:
- CreateAccount
- GetMonthlySummary
- ParseOfxFile

### 4) Propriedades
Usar PascalCase em propriedades públicas.

Exemplos:
- AccountId
- TotalAmount
- DueDate

### 5) Campos privados
Usar camelCase com prefixo underscore quando necessário.

Exemplo:
- _dbContext
- _logger

### 6) Constantes
Usar PascalCase ou UPPER_SNAKE_CASE conforme convenção local, mas manter consistência.

Exemplo:
- DefaultPageSize
- MAX_RETRY_COUNT

---

## Convenções de projeto .NET

### 1) Use records quando fizer sentido
Records podem ser usados para DTOs e objetos imutáveis, especialmente em contratos de aplicação.

### 2) Preferir tipos explícitos
Evitar `object`, `dynamic` e `var` em excesso quando a intenção não for clara.

### 3) Use DTOs para entrada e saída da API
Não expor entidades de domínio diretamente na camada de API.

### 4) Use interfaces somente quando houver ganho real
Não criar interface para tudo.

Quando houver necessidade real de abstração, usar nomes claros e específicos.

Exemplo:
- IAccountRepository
- ICategoryRuleService
- IImportParser

### 5) Use serviços de aplicação e handlers por feature
Para casos de uso, preferir organização por feature, com comando/handler ou caso de uso bem nomeado.

Exemplo:
- CreateAccountCommand
- CreateAccountCommandHandler
- GetMonthlySummaryQuery
- GetMonthlySummaryQueryHandler

### 6) Use repositórios para persistência
Empregar repositórios para acesso a dados e não espalhar queries em toda aplicação.

### 7) Evitar anemias de camada
A camada de domínio deve ter lógica relevante quando ela fizer sentido, mas não virar um “bag of entities” sem comportamento.

---

## Regras sobre fluxo de desenvolvimento

### 1) Nunca criar feature sem pensar em domínio
Antes de criar classes e endpoints, definir o que a funcionalidade representa no domínio do Fluxi.

### 2) Não criar entidades sem necessidade
Não inventar modelos genéricos para tudo.

### 3) Separar casos de uso por intenção
Exemplos:
- CreateTransaction
- ImportTransactions
- GetMonthlySummary
- ApplyCategoryRules

### 4) Separar infraestrutura de domínio
Persistência, integração externa e parse de arquivo não devem misturar com regras fundamentais do domínio.

### 5) Tratamento de erros
Usar exceções de domínio ou resultado explícito quando fizer sentido.

Não lançar exceções genéricas sem contexto.

---

## Regras para importação e domínio financeiro

### 1) Importação é infraestrutura e aplicação, não domínio puro
A leitura de OFX/CSV deve ficar em camada de infraestrutura ou aplicação específica, não no coração do domínio.

### 2) Regras financeiras vão para domínio ou aplicação
Se a regra representa a lógica de negócio do Fluxi, ela deve estar na camada correta do domínio/aplicação.

### 3) Nunca espalhar cálculo de fatura e saldo pela API
Esses cálculos devem seguir regra clara e centralizada.

### 4) Idempotência é obrigatória em imports
Importação recorrente deve evitar duplicação por hash.

---

## Regras para documentação e manutenção

### 1) Todo novo recurso precisa ter documentação mínima
Quando um módulo novo for adicionado, revisar:
- regras de negócio
- fluxos relevantes
- schema do banco, se necessário

### 2) Qualquer mudança no domínio exige revisão dos fluxos
Se a regra mudar, revisar o arquivo de regras de negócio e os fluxos relevantes.

### 3) Código deve refletir o modelo de negócio
O nome das classes, métodos e endpoints deve refletir o domínio financeiro do Fluxi, e não apenas estruturas técnicas.

---

## Proibido

- criar abstrações sem necessidade
- criar serviços genéricos para tudo
- misturar regras de negócio com infraestrutura
- criar namespaces sem sentido
- usar nomes técnicos vagos como `Manager`, `Helper`, `Util`, `Common`
- deixar lógica de cálculo financeiro espalhada em controle de API
- duplicar regras de negócio em front-end
- criar projetos separados para coisas que ainda não justificam separação

---

## Padrão preferido do Fluxi

O Fluxi deve seguir este padrão:

- Clean Architecture
- monólito modular
- vertical slices por feature
- nomes claros e específicos
- regras de domínio centralizadas
- API como entrada
- PostgreSQL único
- formação simples e evolutiva

---

## Resumo prático

Se a IA estiver gerando código para este projeto, ela deve:

1. manter a separação por camada
2. usar nomes expressivos e do domínio
3. organizar por feature e não por técnica genérica
4. deixar regras de negócio no backend
5. evitar abstrações prematuras
6. manter os fluxos e regras de negócio sincronizados
7. construir em etapas pequenas e viáveis

Esse documento deve ser usado como referência principal para qualquer criação de estrutura e geração de código do projeto .NET.
