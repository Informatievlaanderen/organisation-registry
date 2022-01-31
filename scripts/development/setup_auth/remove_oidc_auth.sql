DELETE
FROM OrganisationRegistry.OrganisationRegistry.Configuration
WHERE [Key] LIKE N'OIDCAuth:Authority' ESCAPE '#';

DELETE
FROM OrganisationRegistry.OrganisationRegistry.Configuration
WHERE [Key] LIKE N'OIDCAuth:Developers' ESCAPE '#';

DELETE
FROM OrganisationRegistry.OrganisationRegistry.Configuration
WHERE [Key] LIKE N'OIDCAuth:TokenEndPoint' ESCAPE '#';

DELETE
FROM OrganisationRegistry.OrganisationRegistry.Configuration
WHERE [Key] LIKE N'OIDCAuth:CallbackPath' ESCAPE '#';


