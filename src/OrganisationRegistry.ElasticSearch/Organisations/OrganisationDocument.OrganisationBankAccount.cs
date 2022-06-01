namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using Osc;

public partial class OrganisationDocument
{
    public class OrganisationBankAccount
    {
        public Guid OrganisationBankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public bool IsIban { get; set; }
        public string Bic { get; set; }
        public bool IsBic { get; set; }
        public Period Validity { get; set; }

#pragma warning disable CS8618
        private OrganisationBankAccount()
#pragma warning restore CS8618
        { }

        public OrganisationBankAccount(
            Guid organisationBankAccountId,
            string bankAccountNumber,
            bool isIban,
            string bic,
            bool isBic,
            Period validity)
        {
            OrganisationBankAccountId = organisationBankAccountId;
            BankAccountNumber = bankAccountNumber;
            IsIban = isIban;
            Bic = bic;
            IsBic = isBic;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBankAccount> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationBankAccountId))
                .Text(
                    t => t
                        .Name(p => p.BankAccountNumber)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Boolean(
                    t => t
                        .Name(p => p.IsIban))
                .Text(
                    t => t
                        .Name(p => p.Bic)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Boolean(
                    t => t
                        .Name(p => p.IsBic))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}
