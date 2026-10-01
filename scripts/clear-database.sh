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

echo "Clearing OrganisationRegistry database..."
"${KUBECTL}" exec -n "${NAMESPACE}" mssql-0 -- \
  "${SQLCMD}" \
    -S localhost \
    -U sa \
    -P "${MSSQL_PASSWORD}" \
    -Q "IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'OrganisationRegistry') BEGIN ALTER DATABASE [OrganisationRegistry] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [OrganisationRegistry]; END; CREATE DATABASE [OrganisationRegistry];" \
    -b

echo "Database cleared."

echo "Resetting OpenSearch data..."
"${KUBECTL}" scale statefulset opensearch -n "${NAMESPACE}" --replicas=1
for i in $(seq 1 30); do
  "${KUBECTL}" get pod/opensearch-0 -n "${NAMESPACE}" >/dev/null 2>&1 && break
  sleep 2
done
"${KUBECTL}" wait --for=condition=ready pod/opensearch-0 -n "${NAMESPACE}" --timeout=300s

echo "OpenSearch reset complete."

