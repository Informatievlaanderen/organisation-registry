#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
KUBECONFIG="${KUBECONFIG:-${ROOT_DIR}/.kubeconfig}"
NAMESPACE="${NAMESPACE:-wegwijs-demo}"
KUBECTL="${KUBECTL:-kubectl}"
SQLCMD="${SQLCMD:-/opt/mssql-tools/bin/sqlcmd}"

export KUBECONFIG

wait_for_pod() {
  local pod="$1"
  # the pod may not exist yet while a StatefulSet is being recreated
  for _ in $(seq 1 60); do
    "${KUBECTL}" get "pod/${pod}" -n "${NAMESPACE}" >/dev/null 2>&1 && break
    sleep 2
  done
  "${KUBECTL}" wait --for=condition=ready "pod/${pod}" -n "${NAMESPACE}" --timeout=300s
}

wait_for_pod mssql-0
wait_for_pod opensearch-0

MSSQL_SA_PASSWORD="$(
  "${KUBECTL}" get secret demo-secrets \
    -n "${NAMESPACE}" \
    -o jsonpath='{.data.mssql-sa-password}' | base64 --decode
)"

# --- SQL Server -------------------------------------------------------------

read -r -d '' SQL <<'EOF' || true
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'OrganisationRegistry')
BEGIN
  ALTER DATABASE [OrganisationRegistry] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  DROP DATABASE [OrganisationRegistry];
END;
CREATE DATABASE [OrganisationRegistry];
EOF

echo "Clearing OrganisationRegistry database..."
"${KUBECTL}" exec -n "${NAMESPACE}" mssql-0 -- \
  "${SQLCMD}" -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -b -Q "${SQL}" \
  || { echo "Database clear failed" >&2; exit 1; }
echo "Database cleared."

# --- OpenSearch -------------------------------------------------------------

echo "Resetting OpenSearch data..."
"${KUBECTL}" exec -n "${NAMESPACE}" opensearch-0 -- \
  curl -sf -X DELETE 'http://localhost:9200/_all' >/dev/null \
  || { echo "OpenSearch wipe failed" >&2; exit 1; }
"${KUBECTL}" exec -n "${NAMESPACE}" opensearch-0 -- \
  curl -sf 'http://localhost:9200/_cluster/health?wait_for_status=yellow&timeout=60s' >/dev/null \
  || { echo "OpenSearch health check failed" >&2; exit 1; }
echo "OpenSearch reset complete."

