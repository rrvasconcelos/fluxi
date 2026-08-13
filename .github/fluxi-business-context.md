# Fluxi — contexto de negócio

Este documento registra o contexto de negócio e as decisões de produto do
Fluxi. Consulte-o antes de propor ou implementar entidades, fluxos ou regras
do domínio.

## O que é o Fluxi

Fluxi é um app pessoal de controle financeiro. Não é um produto comercial
(ainda); é um projeto para uso próprio, que pode crescer no futuro.

### Problema que resolve

O usuário utiliza múltiplos bancos: recebe o salário e paga contas fixas em
um banco, enquanto mantém os gastos do dia a dia, especialmente do cartão de
crédito, em outro. Atualmente controla tudo em planilha e lança cada
transação manualmente todos os meses, o que torna difícil manter o controle
atualizado.

O usuário não quer conectar seu login bancário a aplicativos de terceiros.
Open Finance por meio de agregadores, como Pluggy, foi considerado e
descartado por esse motivo. A solução deve importar arquivos que o próprio
usuário baixa do banco (OFX, CSV ou PDF), sem compartilhar credenciais.

### Bancos e formatos disponíveis

- **Nubank**: exporta fatura em OFX e CSV, apenas quando fechada. É a fonte
  mais fácil de automatizar e será usada na v1.
- **Bradesco**: a conta corrente exporta OFX pelo internet banking, somente
  no navegador. A fatura do cartão está disponível apenas em PDF.
- **Sofisa**: oferece apenas PDF, sem exportação estruturada conhecida.

O app deve suportar múltiplos métodos de importação por conta
(`import_method`: `ofx`, `csv`, `pdf` ou `manual`), e não apenas um método
global.

## Decisões de produto

1. **Múltiplas contas e fontes**: o sistema deve tratar N contas desde o
   início. Conta é uma lista de `account`, não uma única conta fixa.
2. **Fatura não é mês-calendário**: cartão de crédito fecha por `invoice`,
   com `closing_date` e `due_date` próprios. A visão mensal soma o que vence
   naquele mês, não o que foi comprado nele.
3. **Parcelamento automático**: uma compra parcelada manualmente (`purchase`)
   deve gerar sozinha as N parcelas futuras (`transaction`), distribuídas nas
   próximas faturas. Compras importadas do Nubank já vêm parcela por parcela e
   não precisam desse tratamento.
4. **Categorização por regras**: `category_rule` deve casar um padrão de texto,
   como `IFOOD`, na descrição da transação e aplicar a categoria
   correspondente, reduzindo o trabalho manual recorrente.
5. **Visão mensal como fluxo central**: mostrar por mês o salário líquido, o
   total de contas fixas que vencem, o total da fatura de cartão que vence e
   quanto sobra. A fatura em aberto, ainda em fechamento, deve aparecer
   separadamente.
6. **Simulação de compra**: a feature diferencial é o `goal`, que usa o
   histórico real de receitas e despesas para simular se e quando uma compra,
   como carro ou apartamento, cabe no orçamento, à vista ou financiada.
7. **Deduplicação de importações**: toda `transaction` deve ter um `hash`
   baseado em data, valor e descrição para evitar duplicar lançamentos ao
   reimportar um período.

## Escopo inicial sugerido

- Importar OFX do Nubank.
- Permitir cadastro manual do restante.
- Aplicar categorização por regra.
- Exibir uma visão mensal simples.
- Implementar o parser de OFX antes do parser de PDF, que fica para a v2.