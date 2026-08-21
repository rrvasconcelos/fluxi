# Fluxi — regras de negócio do domínio que precisam ser validadas antes da aplicação

Este arquivo reúne as regras que pertencem ao domínio da v1 e que devem ser validadas em entidades e testes unitários antes de avançar para Application, Infrastructure e API.

A ideia é separar o que é regra de negócio do domínio do que é regra de infraestrutura, persistência, autorização, repositório ou orquestração de casos de uso.

## Como ler este documento

- Se a regra diz respeito à entidade, ao valor, ao ciclo do domínio e ao comportamento esperado do agregado, ela pertence ao domínio.
- Se a regra depende de banco, consulta, concorrência, unicidade global, repositórios ou importações com parsers externos, ela normalmente fica em Application/Infrastructure.

## Status atual do domínio

Já existem regras implementadas para:

- Account
- Category
- CategoryRule
- Transaction

Falta concluir as regras do restante do modelo financeiro da v1:

- Invoice
- Income
- FixedExpense

## 1) Account

### Regras do domínio

A entidade Account deve validar e preservar estes comportamentos:

- nome é obrigatório e não pode ser vazio, nulo ou só espaços;
- banco é obrigatório e não pode ser vazio, nulo ou só espaços;
- tipo da conta deve ser um valor suportado (`checking` ou `credit_card`);
- método de importação deve ser um valor suportado (`ofx`, `csv`, `manual`);
- toda conta começa com status `active`;
- `Activate()` deve manter a conta ativa;
- `Deactivate()` deve manter a conta inativa;
- ativar uma conta já ativa deve ser uma operação idempotente;
- inativar uma conta já inativa deve ser uma operação idempotente;
- `ChangeName()` deve validar o nome antes de alterar;
- `ChangeBank()` deve validar o banco antes de alterar;
- `ChangeImportMethod()` deve aceitar somente valores suportados;

### Regras que não pertencem ao domínio

Essas regras podem exigir persistência e consulta, então não devem ser duplicadas no agregado:

- garantir unicidade do nome entre contas;
- impedir alteração de tipo se houver transações relacionadas;
- impedir importação ou novo lançamento em conta inativa;
- excluir fisicamente a conta; no domínio a regra é manter histórico e inativar;
- garantir consistência concorrente em banco de dados.

## 2) Category

### Regras do domínio

A entidade Category deve garantir:

- nome da categoria é obrigatório;
- nome não pode ser vazio, nulo ou só espaços;
- nome pode ser alterado por `ChangeName()` desde que permaneça válido;
- a categoria representa a classificação do gasto ou da receita;
- a categoria não deve conhecer regras de importação nem persistência;

### Regras que não pertencem ao domínio

- unicidade do nome em banco;
- validação de duplicidade entre categorias em cenários concorrentes;
- regras de ordenação ou uso por usuário; isso é de aplicação.

## 3) CategoryRule

### Regras do domínio

A entidade CategoryRule deve garantir:

- `pattern` é obrigatório;
- `pattern` não pode ser vazio, nulo ou só espaços;
- `priority` não pode ser negativo;
- a regra deve casar um padrão em texto de transação, de forma case-insensitive;
- `Matches(string text)` deve retornar `true` quando o padrão estiver presente na descrição;
- se houver múltiplas regras, a de maior prioridade deve vencer; essa comparação de prioridade pode ocorrer em aplicação, mas a regra de prioridade e ordenação pertence à regra de negócio do domínio;

### Regra importante do domínio

A regra de categorização precisa ser estável e previsível:

- padrão textual + categoria + prioridade
- a comparação deve ser determinística
- regras com maior prioridade devem ter preferência sobre regras menores

## 4) Transaction

### Regras do domínio

A entidade Transaction deve garantir:

- `accountId` obrigatória;
- `description` obrigatória;
- `amount` deve ser maior que zero;
- `type` deve ser um valor suportado (`income` ou `expense`);
- `origin` deve ser um valor suportado (`imported` ou `manual`);
- `hash` deve ser gerado a partir de dados de identificação da transação para evitar duplicação;
- `hash` deve ser calculado usando ao menos: conta, data, valor e descrição;
- `CategoryId` pode ser nulo até a transação ser categorizada;
- `InvoiceId` pode ser nulo para contas correntes ou movimentações sem vínculo fiscal;
- `AssignCategory(Guid categoryId)` deve aceitar somente um identificador válido;
- transação deve ter data válida;
- a transação representa a unidade central de movimentação financeira do sistema;

### Regras de deduplicação e consistência

- hash deve permitir detectar importação repetida do mesmo período; 
- a comparação de hash deve considerar a conta, a data, o valor e a descrição;
- o domínio deve manter a transação como entidade principal e não depender de camada externa para calcular a identidade dela;

### Regras que não pertencem ao domínio

- buscar duplicatas em banco para decidir se a transação já existe;
- a regra de sincronização com importadores e arquivos externos;
- validação de saldo ou orçamento em cima da transação;
- lógica de leitura de dados de repositório para determinar duplicação em produção.

## 5) Invoice

### Regras do domínio

A entidade Invoice representa a fatura do cartão de crédito e deve garantir:

- `accountId` obrigatório;
- `reference` obrigatório, por exemplo `2026-08`;
- `closing_date` obrigatório;
- `due_date` obrigatório;
- `status` deve ser um valor válido (`open`, `closed`, `paid`);
- `total_amount` pode ser nulo no início e depois atualizado;
- `closing_date` e `due_date` devem ser datas válidas;
- `due_date` não pode ser anterior a `closing_date`;
- a fatura não é o mês calendário; ela é o ciclo do cartão;
- o ciclo de fatura deve ser independente do calendário do mês;
- a fatura deve manter a visão da dívida do cartão por ciclo de cobrança;

### Comportamento esperado do agregado

- `Create()` deve aceitar dados válidos e iniciar a fatura com status consistente;
- a fatura pode ser fechada ou paga conforme o ciclo do cartão;
- em domínio, a entidade deve proteger a validade das datas e do status;

### Regras que não pertencem ao domínio

- automatizar a abertura de fatura a partir de contas e transações;
- calcular o total da fatura a partir do banco;
- sincronizar status com importação e movimentação real;
- consultar concorrência para evitar inconsistência de datas.

## 6) Income

### Regras do domínio

A entidade Income representa renda recorrente e deve garantir:

- `accountId` obrigatório;
- `description` obrigatória;
- `amount` deve ser maior que zero;
- `receipt_day` deve ser um dia válido do mês, geralmente de 1 a 31;
- `is_recurring` deve ser booleano; por padrão, `true`;
- uma renda recorrente é parte da visão mensal e deve ser somada ao fluxo do mês;
- a renda não deve depender de importação; ela é cadastrada manualmente;

### Regras de negócio centrais

- renda deve ser tratada como fluxo financeiro positivo;
- a data de recebimento não necessariamente é a data da transação; o importante é o dia de recebimento no mês;
- o nome da renda deve ser legível e sem informações vazias;

### Regras que não pertencem ao domínio

- cálculos específicos de calendário do mês para um período de referência;
- relacionamento com transações importadas;
- regras de calendário fiscal e atualizações de recebimentos automáticos.

## 7) FixedExpense

### Regras do domínio

A entidade FixedExpense representa despesa recorrente fixa e deve garantir:

- `description` obrigatória;
- `amount` maior que zero;
- `due_day` deve ser válido, geralmente 1 a 31;
- `notes` é opcional;
- a despesa fixa representa um custo que se repete mensalmente no fluxo do usuário;
- reduz o custo do mês em forma de gasto recorrente;

### Regras de negócio centrais

- figura como conta fixa mensal; exemplos: aluguel, internet, assinatura, academia;
- deve existir independentemente da importação de transações;
- deve ser tratada como gasto recorrente e parte da visão mensal;

### Regras que não pertencem ao domínio

- cálculo de saldo do mês usando o dia de vencimento real;
- regras de ordenação, filtro ou agrupamento por categoria externa;
- integração com importação e transações.

## 8) Regras compartilhadas de domínio

Essas regras atravessam várias entidades e devem ser observadas em conjunto:

- o sistema trata múltiplas contas e múltiplas fontes;
- a visão mensal é o centro do produto;
- transação é a unidade central de movimentação financeira;
- conta e cartão devem manter histórico sem exclusão física;
- categoria e regra de categoria fazem parte do processo de organização do fluxo financeiro;
- a fatura de cartão é um ciclo de cobrança e não um mês calendário;
- renda e despesas fixas compõem o fluxo mensal, independentemente de importação;
- importação de extrato não substitui o domínio do fluxo financeiro; ela apenas alimenta o sistema.

## 9) O que deve ser validado em testes de domínio

Antes de avançar, cada entidade deve ter testes que cubram:

### Account

- criação com dados válidos;
- nome inválido lança exceção;
- banco inválido lança exceção;
- tipo inválido lança exceção;
- método de importação inválido lança exceção;
- ativar e desativar ficam consistentes;
- mudança de nome e banco são validadas.

### Category

- criação de categoria válida;
- nome nulo ou vazio lança exceção;
- mudança de nome válida;
- mudança de nome inválida lança exceção.

### CategoryRule

- criação válida;
- pattern nulo/vazio lança exceção;
- priority negativo lança exceção;
- `Matches()` retorna `true` para padrão em caixa diferente;
- `Matches()` retorna `false` quando não há casamento.

### Transaction

- criação válida;
- conta vazia lança exceção;
- descrição vazia lança exceção;
- valor zero ou negativo lança exceção;
- tipo inválido lança exceção;
- origem inválida lança exceção;
- `AssignCategory()` com Id vazio lança exceção;
- hash é gerado;
- a entidade mantém categoria e invoice opcionais de forma controlada.

### Invoice

- criação válida;
- conta obrigatória;
- data inválida; 
- `due_date` anterior a `closing_date` lança exceção;
- status inválido lança exceção;
- total pode ser atualizado se necessário;

### Income

- criação válida;
- accountId inválido lança exceção;
- description vazia lança exceção;
- amount inválido lança exceção;
- receipt_day fora do intervalo lança exceção;

### FixedExpense

- criação válida;
- description vazia lança exceção;
- amount inválido lança exceção;
- due_day fora do intervalo lança exceção;
- notes opcional é preservado.

## 10) Checklist do domínio pronto

- [ ] Account validado e coerente
- [ ] Category validado e coerente
- [ ] CategoryRule validado e coerente
- [ ] Transaction validado e coerente
- [ ] Invoice validado e coerente
- [ ] Income validado e coerente
- [ ] FixedExpense validado e coerente
- [ ] Domínio sem regras de infraestrutura misturadas
- [ ] Testes de domínio verdes
- [ ] Pronto para Application

## Regra final

Antes de avançar para Application, o domínio precisa estar estável, coerente e capaz de expressar o problema real do Fluxi sem depender de banco, API, parser ou persistência para validar suas regras de negócio.

Se uma regra depende de consulta a dados externos, ela não deve ser “resolvida” dentro do domínio; ela deve ser movida para a camada correta da arquitetura.
