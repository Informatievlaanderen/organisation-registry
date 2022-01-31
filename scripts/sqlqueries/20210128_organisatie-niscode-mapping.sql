with validNisCodes as (
	select OrganisationId, KeyValue 
	from backoffice.OrganisationKeyList okl
	where KeyTypeName = 'NIS'
	and (okl.ValidFrom is null or okl.ValidFrom <= GETDATE())
    and (okl.ValidTo is null or okl.ValidTo >= GETDATE())
),
validOrganisations as (
select Id, Name, Ovonumber
from Backoffice.OrganisationDetail od
	where (od.ValidFrom is null or od.ValidFrom <= GETDATE())
    and (od.ValidTo is null or od.ValidTo >= GETDATE())
)
select Name, OvoNumber, nis.KeyValue as NISCode
from validOrganisations od
inner join validNisCodes nis on od.Id = nis.OrganisationId
order by NISCode
