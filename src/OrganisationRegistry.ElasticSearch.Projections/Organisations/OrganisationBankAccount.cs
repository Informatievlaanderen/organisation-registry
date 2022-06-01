namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using Common;
using ElasticSearch.Organisations;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Change;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;

public class OrganisationBankAccount :
    BaseProjection<OrganisationBankAccount>,
    IElasticEventHandler<OrganisationBankAccountAdded>,
    IElasticEventHandler<KboOrganisationBankAccountAdded>,
    IElasticEventHandler<KboOrganisationBankAccountRemoved>,
    IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
    IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
    IElasticEventHandler<OrganisationBankAccountUpdated>,
    IElasticEventHandler<OrganisationTerminated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{
    public OrganisationBankAccount(
        ILogger<OrganisationBankAccount> logger)
        : base(logger)
    {
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountAdded> message)
    {
        return await AddBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Body.BankAccountNumber, message.Body.IsIban, message.Body.Bic, message.Body.IsBic, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountAdded> message)
    {
        return await AddBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Body.BankAccountNumber, message.Body.IsIban, message.Body.Bic, message.Body.IsBic, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountRemoved> message)
    {
        return await RemoveBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Number, message.Timestamp);
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        return await RemoveBankAccounts(message.Body.OrganisationId, message.Body.OrganisationBankAccountIdsToCancel, message.Number, message.Timestamp);
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                foreach (var bankAccountId in message.Body.OrganisationBankAccountIdsToTerminate)
                {
                    var organisationBankAccount = document.BankAccounts.Single(x => x.OrganisationBankAccountId == bankAccountId);
                    organisationBankAccount.Validity.End = message.Body.DateOfTermination;
                }
            }
        ).ToAsyncResult();
    }

    private static async Task<IElasticChange> AddBankAccount(Guid organisationId, Guid organisationBankAccountId, string bankAccountNumber, bool isIban, string bic, bool isBic, DateTime? validFrom, DateTime? validTo, int changeId, DateTimeOffset timestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            organisationId,
            document =>
            {
                document.ChangeId = changeId;
                document.ChangeTime = timestamp;

                document.BankAccounts.RemoveExistingListItems(x =>
                    x.OrganisationBankAccountId == organisationBankAccountId);

                document.BankAccounts.Add(
                    new OrganisationDocument.OrganisationBankAccount(
                        organisationBankAccountId,
                        bankAccountNumber,
                        isIban,
                        bic,
                        isBic,
                        Period.FromDates(validFrom, validTo)));
            }
        ).ToAsyncResult();
    }

    private static async Task<IElasticChange> RemoveBankAccount(Guid organisationId, Guid organisationBankAccountId, int changeId, DateTimeOffset timestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            organisationId,
            document =>
            {
                document.ChangeId = changeId;
                document.ChangeTime = timestamp;

                document.BankAccounts.RemoveExistingListItems(x =>
                    x.OrganisationBankAccountId == organisationBankAccountId);
            }
        ).ToAsyncResult();
    }

    private static async Task<IElasticChange> RemoveBankAccounts(Guid organisationId, List<Guid> organisationBankAccountIdsToCancel, int changeId, DateTimeOffset timestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            organisationId,
            document =>
            {
                document.ChangeId = changeId;
                document.ChangeTime = timestamp;

                foreach (var organisationBankAccountId in organisationBankAccountIdsToCancel)
                {
                    document.BankAccounts.RemoveExistingListItems(x =>
                        x.OrganisationBankAccountId == organisationBankAccountId);
                }
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountUpdated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.BankAccounts.RemoveExistingListItems(x => x.OrganisationBankAccountId == message.Body.OrganisationBankAccountId);

                document.BankAccounts.Add(
                    new OrganisationDocument.OrganisationBankAccount(
                        message.Body.OrganisationBankAccountId,
                        message.Body.BankAccountNumber,
                        message.Body.IsIban,
                        message.Body.Bic,
                        message.Body.IsBic,
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var accountsToTerminate =
                    message.Body.FieldsToTerminate.BankAccounts
                        .Union(message.Body.KboFieldsToTerminate.BankAccounts);

                foreach (var (key, value) in accountsToTerminate)
                {
                    var organisationBankAccount =
                        document
                            .BankAccounts
                            .Single(x => x.OrganisationBankAccountId == key);

                    organisationBankAccount.Validity.End = value;
                }
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var accountsToTerminate =
                    message.Body.FieldsToTerminate.BankAccounts
                        .Union(message.Body.KboFieldsToTerminate.BankAccounts);

                foreach (var (key, value) in accountsToTerminate)
                {
                    var organisationBankAccount =
                        document
                            .BankAccounts
                            .Single(x => x.OrganisationBankAccountId == key);

                    organisationBankAccount.Validity.End = value;
                }
            }
        ).ToAsyncResult();
    }
}
