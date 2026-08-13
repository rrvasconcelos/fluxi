# Fluxo de importação

Este fluxo representa o processo principal de importação de dados financeiros para o Fluxi.

```mermaid
flowchart TD
    A[Usuário acessa a conta] --> B[Escolhe importar extrato]
    B --> C[Seleciona arquivo OFX / CSV / manual]
    C --> D[API valida o arquivo]
    D --> E{Arquivo válido?}
    E -- Não --> F[Retorna erro e mostra mensagem]
    F --> G[Fim]
    E -- Sim --> H[Sistema prepara transações]
    H --> I[Normaliza dados]
    I --> J[Calcula hash para evitar duplicatas]
    J --> K[Persiste transações]
    K --> L[Aplica regras de categoria]
    L --> M[Atualiza saldo e visão mensal]
    M --> N[Fim]
```

## Observações

- O objetivo é reduzir esforço manual.
- A importação deve ser idempotente.
- O hash evita duplicar lançamentos ao reimportar um período já processado.
- A v1 prioriza OFX e CSV, com suporte manual.
