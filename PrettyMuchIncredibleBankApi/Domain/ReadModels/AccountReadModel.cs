﻿using EventFlow.Aggregates;
using EventFlow.ReadStores;
using PMI.Domain.AccountModel;
using PMI.Domain.Events;
using PMI.Domain.TransactionModel;

namespace PMI.Domain.ReadModels;

public class AccountReadModel : IReadModel,
    IAmReadModelFor<AccountAggregate, AccountId, AccountCreatedEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, MoneyDepositedEvent>,
    IAmReadModelFor<AccountAggregate, AccountId, MoneyWithdrawnEvent>
{
    public AccountId AccountId { get; private set; }
    public decimal Balance { get; private set; }
    public List<Transaction> Transactions { get; private set; }

    private int Version { get; set; }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, AccountCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = (AccountId)domainEvent.GetIdentity();
        Balance = 0;
        Transactions = [];
        Version = domainEvent.AggregateSequenceNumber;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, MoneyDepositedEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = (AccountId)domainEvent.GetIdentity();
        Transactions.Add(domainEvent.AggregateEvent.Transaction);
        Balance += domainEvent.AggregateEvent.Transaction.Amount;
        Version = domainEvent.AggregateSequenceNumber;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<AccountAggregate, AccountId, MoneyWithdrawnEvent> domainEvent, CancellationToken cancellationToken)
    {
        AccountId = (AccountId)domainEvent.GetIdentity();
        Transactions.Add(domainEvent.AggregateEvent.Transaction);
        Balance -= domainEvent.AggregateEvent.Transaction.Amount;
        Version = domainEvent.AggregateSequenceNumber;
        return Task.CompletedTask;
    }

    public AccountStatement ToAccountStatement()
    {
        return new AccountStatement(AccountId.Value, Balance, Transactions, Version);
    }
}