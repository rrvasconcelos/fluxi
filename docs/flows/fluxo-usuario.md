# Fluxo do usuário

Visão geral da jornada do usuário dentro do Fluxi.

```mermaid
flowchart TD
    A[Usuário abre o app] --> B[Visualiza contas e saldo]
    B --> C[Consulta faturas e vencimentos]
    C --> D[Registra ou importa transações]
    D --> E[Categoria automática ou manual]
    E --> F[Consulta visão mensal]
    F --> G[Analisa receitas, despesas e sobra]
    G --> H{Precisa ajustar alguma coisa?}
    H -- Sim --> I[Editar categoria / conta / lançamento]
    I --> F
    H -- Não --> J[Fim]
```

## Observações

- A experiência principal gira em torno da visão mensal.
- O fluxo deve ser simples e rápido para uso diário.
- O usuário não deve precisar categorizar tudo manualmente.
