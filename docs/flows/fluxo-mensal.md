# Fluxo da visão mensal

Fluxo de cálculo e visualização do mês financeiro.

```mermaid
flowchart TD
    A[Sistema recebe período] --> B[Busca rendas do mês]
    B --> C[Busca despesas fixas]
    C --> D[Busca transações do período]
    D --> E[Busca faturas de cartão vencendo]
    E --> F[Calcula total de receitas]
    F --> G[Calcula total de despesas fixas]
    G --> H[Calcula total de cartão]
    H --> I[Calcula saldo restante]
    I --> J[Exibe painel mensal]
    J --> K[Mostra insights e alertas]
    K --> L[Fim]
```

## Observações

- A visão mensal é o coração do produto.
- O sistema deve considerar o que vence no mês, e não necessariamente o que foi comprado no mês.
- O objetivo é mostrar quanto sobra antes de decisões financeiras mais complexas.
