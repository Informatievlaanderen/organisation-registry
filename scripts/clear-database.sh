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
    -Q "ALTER DATABASE [OrganisationRegistry] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [OrganisationRegistry]; CREATE DATABASE [OrganisationRegistry];" \
    -b

echo "Database cleared."
