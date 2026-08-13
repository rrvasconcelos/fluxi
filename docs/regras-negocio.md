# Fluxi — Regras de negócio da v1

Este arquivo é o ponto central de decisões de negócio do projeto.

Toda regra aqui descrita deve ser refletida em:
- schema do banco
- fluxo de negócio/documentação
- regras de validação da API
- testes de domínio e integração

## Regra 1 — Múltiplas contas e fontes
O sistema deve permitir cadastrar múltiplas contas e cartões de diferentes bancos.

Cada conta deve ter:
- nome
- banco
- tipo (`checking` ou `credit_card`)
- método de importação (`ofx`, `csv`, `manual`)

Cada transação deve estar vinculada a uma conta.

## Regra 2 — Cartão de crédito por fatura
O cartão de crédito deve ser tratado por fatura, não por mês calendário.

A lógica de fatura deve considerar:
- data de fechamento
- data de vencimento
- total da fatura
- status da fatura (`open`, `closed`, `paid`)

A visão mensal deve considerar o que vence naquele mês e não somente o que foi comprado naquele mês.

## Regra 3 — Importação sem credenciais bancárias
O sistema deve importar arquivos fornecidos pelo usuário, sem pedir login bancário em terceiros.

Formatos aceitos na v1:
- OFX
- CSV
- manual

PDF fica fora da v1, salvo evolução posterior.

## Regra 4 — Duplicatas evitadas por hash
Toda transação deve ter um hash calculado a partir de dados essenciais para evitar duplicatas ao importar o mesmo período novamente.

A deduplicação deve ser feita por:
- data
- valor
- descrição
- conta

## Regra 5 — Categorizações por regra automática
O sistema deve aplicar categorias automaticamente com base em regras configuradas pelo usuário.

Regras têm:
- padrão textual
- categoria associada
- prioridade

Se houver múltiplas regras, a de maior prioridade vence.

## Regra 6 — Visão mensal como coração do produto
A visão mensal deve mostrar, ao menos:
- salário ou renda líquida do período
- total de despesas fixas
- total de fatura de cartão que vence
- saldo restante

A visão mensal deve funcionar como principal painel do produto.

## Regra 7 — Despesas fixas são recorrentes
Despesas fixas devem ser configuradas independentemente de importação.

Exemplos:
- aluguel
- internet
- assinatura
- academia
- outras parcelas fixas

## Regra 8 — Renda recorrente também existe
O sistema deve permitir cadastrar fontes de renda recorrente, como salário ou rendimento.

A renda deve poder ser usada na composição da visão mensal.

## Regra 9 — Transação é a unidade central
Toda movimentação financeira deve ser registrada como transação.

A transação centraliza:
- conta
- data
- descrição
- valor
- tipo (`income` ou `expense`)
- categoria
- origem (`imported` ou `manual`)

## Regra 10 — MVP simples e evolutivo
A v1 deve resolver o problema principal sem criar arquitetura complexa demais.

A v1 deve priorizar:
- contas
- transações
- categorias e regras
- importação básica
- visão mensal

Itens como simulação de compra, parcelamento automático sofisticado e PDF complexos ficam para versões futuras.

---

## Regra de atualização obrigatória

Toda alteração neste arquivo deve ser acompanhada da revisão dos fluxos existentes em:
- docs/flows/fluxo-importacao.md
- docs/flows/fluxo-usuario.md
- docs/flows/fluxo-mensal.md
- docs/flows/fluxo-categoria.md

Se uma regra de negócio mudar, o fluxo que a envolve também deve ser atualizado para refletir a nova realidade.

Este documento é a fonte de verdade do negócio, e os fluxos são a representação visual dessa regra.
