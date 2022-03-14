# ElasticSearch Projections

## Rebuilding the projections

ElasticSearch Projections provides a read client and a write client. This allows us to rebuild the index on a new index with the write client, while the read client keeps providing the old index.

To rebuild the projections:

- Disable the projections

```
UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:ElasticSearchProjectionsAvailable' ESCAPE '#';
```

- Stop the OrganisationRegistry.ElasticSearch.Projections process in AWS, to make sure the above configuration setting is picked up.

- Set the ElasticSearch:OrganisationsWriteIndex to a new version, eg: `or-organisations-<unique identifier>`. We usually use today's date as the identifier (see example).
```
-- EXAMPLE, REPLACE DATE
UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'or-beta-organisations-2022-03-14'
WHERE [Key] LIKE N'ElasticSearch:OrganisationsWriteIndex' ESCAPE '#';
```
- Set the projection states to `-1` of the projections you want to rebuild.
```
-- EXAMPLE, REPLACE ID --

UPDATE [organisation-registry].Backoffice.ProjectionStateList
SET EventNumber = -1
WHERE Id = N'<ID OF PROJECTION>';
```

- Enable the projections

```
UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:ElasticSearchProjectionsAvailable' ESCAPE '#';
```

- Stop the OrganisationRegistry.ElasticSearch.Projections process in AWS, to make sure the above configuration setting is picked up.

- Wait while the projections are being rebuilt, progress can be followed in the `ProjectionStates` table.

- When the projections have caught up, point the Read index to the Write index.

```
-- EXAMPLE, REPLACE DATE --

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'or-beta-organisations-2022-03-14'
WHERE [Key] LIKE N'ElasticSearch:OrganisationsWriteIndex' ESCAPE '#';
```

- Restart OrganisationRegistry.Api, to make sure the read index setting is picked up.
