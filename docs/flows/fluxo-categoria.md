# Fluxo de categorização

Fluxo para categorizar transações automaticamente ou manualmente.

```mermaid
flowchart TD
    A[Transação importada ou criada] --> B[Extrai descrição]
    B --> C[Busca regras de categoria]
    C --> D{Existe regra que bate?}
    D -- Sim --> E[Aplica categoria da regra com maior prioridade]
    E --> F[Salva transação categorizada]
    F --> G[Fim]
    D -- Não --> H[Transação fica sem categoria]
    H --> I[Usuário revisa manualmente]
    I --> J[Salva categoria manual]
    J --> K[Fim]
```

## Observações

- As regras devem buscar padrões de texto.
- A prioridade define qual categoria deve vencer quando houver múltiplas regras.
- Esse fluxo reduz trabalho manual e melhora a experiência no uso recorrente.
