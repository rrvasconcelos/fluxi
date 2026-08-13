# Fluxi — Schema v1 enxuta

Versão inicial do banco do Fluxi, pensada para resolver o problema real sem
explodir em complexidade. A ideia da v1 é entregar uma base útil para:

- controlar contas e cartões
- importar transações de OFX/CSV/manual
- categorizar automaticamente
- acompanhar rendas e despesas fixas
- ver visão mensal sem complicar demais

A v1 não inclui compra parcelada complexa, simulação de objetivo de compra,
ni nem suporte amplo a PDF. Isso fica para depois.

---

## Objetivo da v1

A funcionalidade principal é:

1. Você cadastra uma ou mais contas
2. Importa transações
3. As transações entram no banco com categoria e origem
4. A aplicação calcula no mês: renda, despesas fixas, dívida de cartão e
   saldo restante

Essa abordagem resolve o problema real sem transformar o projeto em um ERP
financeiro.

---

## `account`

Representa cada conta ou cartão. É a base do sistema.

```sql
CREATE TABLE account (
  id            UUID PRIMARY KEY,
  name          TEXT NOT NULL,
  bank          TEXT NOT NULL,
  type          TEXT NOT NULL,   -- 'checking' ou 'credit_card'
  import_method TEXT NOT NULL    -- 'ofx', 'csv', 'manual'
);
```

---

## `category`

Categorias usadas para agrupar gastos e receitas.

```sql
CREATE TABLE category (
  id   UUID PRIMARY KEY,
  name TEXT NOT NULL UNIQUE
);
```

---

## `category_rule`

Regra automatizada para categorizar transações com base na descrição.

```sql
CREATE TABLE category_rule (
  id          UUID PRIMARY KEY,
  pattern     TEXT NOT NULL,
  category_id UUID NOT NULL REFERENCES category(id),
  priority    INT NOT NULL DEFAULT 0
);
```

---

## `invoice`

Fatura do cartão de crédito. Mantida apenas como suporte para a visão do
cartão e para relacionar lançamentos a um ciclo de cobrança.

```sql
CREATE TABLE invoice (
  id           UUID PRIMARY KEY,
  account_id   UUID NOT NULL REFERENCES account(id),
  reference    TEXT NOT NULL,      -- ex: "2026-08"
  closing_date DATE NOT NULL,
  due_date     DATE NOT NULL,
  total_amount DECIMAL(12,2),
  status       TEXT NOT NULL       -- 'open', 'closed', 'paid'
);
```

---

## `transaction`

Cada lançamento individual. É a tabela central do sistema.

```sql
CREATE TABLE transaction (
  id                UUID PRIMARY KEY,
  account_id        UUID NOT NULL REFERENCES account(id),
  invoice_id        UUID REFERENCES invoice(id),
  category_id       UUID REFERENCES category(id),
  date              DATE NOT NULL,
  description       TEXT NOT NULL,
  amount            DECIMAL(12,2) NOT NULL,
  type              TEXT NOT NULL,      -- 'income' ou 'expense'
  origin            TEXT NOT NULL,      -- 'imported' ou 'manual'
  hash              TEXT UNIQUE,
  created_at        TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

Observações:
- `hash` serve para evitar duplicatas ao reimportar
- `invoice_id` pode ficar nulo para contas correntes e para despesas sem
  vínculo fiscal
- `category_id` pode ficar nulo até a transação ser categorizada

---

## `income`

Renda recorrente, como salário ou rendimento mensal.

```sql
CREATE TABLE income (
  id           UUID PRIMARY KEY,
  account_id   UUID NOT NULL REFERENCES account(id),
  description  TEXT NOT NULL,
  amount       DECIMAL(12,2) NOT NULL,
  receipt_day  INT NOT NULL,
  is_recurring BOOLEAN NOT NULL DEFAULT TRUE
);
```

---

## `fixed_expense`

Despesa recorrente fixa, como aluguel, internet, assinatura, academia.

```sql
CREATE TABLE fixed_expense (
  id          UUID PRIMARY KEY,
  description TEXT NOT NULL,
  amount      DECIMAL(12,2) NOT NULL,
  due_day     INT NOT NULL,
  notes       TEXT
);
```

---

## Relacionamentos da v1

- `account` 1—N `transaction`
- `account` 1—N `income`
- `account` 1—N `invoice`
- `invoice` 1—N `transaction`
- `category` 1—N `transaction`
- `category` 1—N `category_rule`

---

## O que ficou fora da v1

Para manter o projeto simples e sustentável, a v1 propositalmente não inclui:

- `purchase` / parcelamento automático
- `installment_number` / `installment_total`
- `goal`
- simulação financeira de compra
- parser de PDF complexo
- suporte massivo a múltiplos formatos de importação de uma vez

Esses itens podem entrar em v2, quando o sistema já estiver resolvendo bem o
problema principal.

---

## Resumo de filosofia

A v1 do Fluxi deve ser útil o suficiente para que você use todos os meses,
sem criar uma base de dados rica demais para começar. O foco é: importar,
organizar, categorizar e ver o que entra e sai.
