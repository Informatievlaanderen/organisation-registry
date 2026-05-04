#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
KUBECONFIG="${KUBECONFIG:-${ROOT_DIR}/.kubeconfig}"
NAMESPACE="${NAMESPACE:-wegwijs-demo}"
KUBECTL="${KUBECTL:-kubectl}"
SQLCMD="${SQLCMD:-/opt/mssql-tools/bin/sqlcmd}"

export KUBECONFIG

MSSQL_PASSWORD="$(
  "${KUBECTL}" get secret demo-secrets \
    -n "${NAMESPACE}" \
    -o jsonpath='{.data.mssql-sa-password}' | base64 --decode
)"
KBO_CERTIFICATE="$(
  "${KUBECTL}" get secret demo-secrets \
    -n "${NAMESPACE}" \
    -o jsonpath='{.data.kbo-certificate}' | base64 --decode
)"

read -r -d '' SQL <<'SQL' || true
SET NOCOUNT ON;

MERGE [OrganisationRegistry].[Configuration] AS target
USING (VALUES
  (
    N'Api:KboV2RegisteredOfficeLocationTypeId',
    N'KBO-veld v2: LocationType Id voor Maatschappelijke zetel',
    N'a7e93f04-0004-0000-0000-000000000001'
  ),
  (
    N'Api:KboV2LegalFormOrganisationClassificationTypeId',
    N'KBO-veld v2: OrganisationClassificationType Id voor Rechtsvorm',
    N'a7e93f06-0006-0000-0000-000000000001'
  ),
  (
    N'Api:KboV2FormalNameLabelTypeId',
    N'KBO-veld v2: LabelType Id voor Formele Benaming',
    N'a7e93f02-0002-0000-0000-000000000004'
  ),
  (
    N'Api:KboMagdaEndpoint',
    N'KBO: endpoint voor MAGDA',
    N'http://wiremock:8080'
  ),
  (
    N'Api:RepertoriumMagdaEndpoint',
    N'Repertorium: endpoint voor Reportorium inschrijvingen',
    N'http://wiremock:8080'
  ),
  (
    N'Api:KboCertificate',
    N'KBO: identificatie van de afzender',
    N'$(KboCertificate)'
  )
) AS source ([Key], [Description], [Value])
ON target.[Key] = source.[Key]
WHEN MATCHED THEN
  UPDATE SET [Description] = source.[Description], [Value] = source.[Value]
WHEN NOT MATCHED THEN
  INSERT ([Key], [Description], [Value])
  VALUES (source.[Key], source.[Description], source.[Value]);

SELECT
  [Key],
  CASE
    WHEN [Key] = N'Api:KboCertificate' THEN CONCAT(N'<base64 certificate, length ', LEN([Value]), N'>')
    ELSE [Value]
  END AS [Value]
FROM [OrganisationRegistry].[Configuration]
WHERE [Key] IN (
  N'Api:KboV2RegisteredOfficeLocationTypeId',
  N'Api:KboV2LegalFormOrganisationClassificationTypeId',
  N'Api:KboV2FormalNameLabelTypeId',
  N'Api:KboMagdaEndpoint',
  N'Api:RepertoriumMagdaEndpoint',
  N'Api:KboCertificate'
)
ORDER BY [Key];
SQL

SQL="${SQL//\$(KboCertificate)/${KBO_CERTIFICATE}}"

echo "Seeding Tilt API configuration values..."
"${KUBECTL}" exec -n "${NAMESPACE}" mssql-0 -- \
  "${SQLCMD}" \
    -S localhost \
    -U sa \
    -P "${MSSQL_PASSWORD}" \
    -d OrganisationRegistry \
    -b \
    -Q "${SQL}"

echo "Restarting API so singleton configuration is reloaded..."
"${KUBECTL}" rollout restart deployment/api -n "${NAMESPACE}"
"${KUBECTL}" rollout status deployment/api -n "${NAMESPACE}" --timeout=300s
