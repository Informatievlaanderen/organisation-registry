namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using Client;
    using Common;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationBankAccount :
        BaseProjection<OrganisationBankAccount>,
        IEventHandler<OrganisationBankAccountAdded>,
        IEventHandler<KboOrganisationBankAccountAdded>,
        IEventHandler<KboOrganisationBankAccountRemoved>,
        IEventHandler<OrganisationBankAccountUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationBankAccount(
            ILogger<OrganisationBankAccount> logger,
            Elastic elastic)
            : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountAdded> message)
        {
            AddBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Body.BankAccountNumber, message.Body.IsIban, message.Body.Bic, message.Body.IsBic, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountAdded> message)
        {
            AddBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Body.BankAccountNumber, message.Body.IsIban, message.Body.Bic, message.Body.IsBic, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountRemoved> message)
        {
            RemoveBankAccount(message.Body.OrganisationId, message.Body.OrganisationBankAccountId, message.Number, message.Timestamp);
        }

        private void AddBankAccount(Guid organisationId, Guid organisationBankAccountId, string bankAccountNumber, bool isIban, string bic, bool isBic, DateTime? validFrom, DateTime? validTo, int changeId, DateTimeOffset timestamp)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = changeId;
            organisationDocument.ChangeTime = timestamp;

            if (organisationDocument.BankAccounts == null)
                organisationDocument.BankAccounts = new List<OrganisationDocument.OrganisationBankAccount>();

            organisationDocument.BankAccounts.RemoveExistingListItems(x =>
                x.OrganisationBankAccountId == organisationBankAccountId);

            organisationDocument.BankAccounts.Add(
                new OrganisationDocument.OrganisationBankAccount(
                    organisationBankAccountId,
                    bankAccountNumber,
                    isIban,
                    bic,
                    isBic,
                    new Period(validFrom, validTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        private void RemoveBankAccount(Guid organisationId, Guid organisationBankAccountId, int changeId, DateTimeOffset timestamp)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = changeId;
            organisationDocument.ChangeTime = timestamp;

            if (organisationDocument.BankAccounts == null)
                organisationDocument.BankAccounts = new List<OrganisationDocument.OrganisationBankAccount>();

            organisationDocument.BankAccounts.RemoveExistingListItems(x =>
                x.OrganisationBankAccountId == organisationBankAccountId);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountUpdated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.BankAccounts == null)
                organisationDocument.BankAccounts = new List<OrganisationDocument.OrganisationBankAccount>();

            organisationDocument.BankAccounts.RemoveExistingListItems(x => x.OrganisationBankAccountId == message.Body.OrganisationBankAccountId);

            organisationDocument.BankAccounts.Add(
                new OrganisationDocument.OrganisationBankAccount(
                    message.Body.OrganisationBankAccountId,
                    message.Body.BankAccountNumber,
                    message.Body.IsIban,
                    message.Body.Bic,
                    message.Body.IsBic,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
