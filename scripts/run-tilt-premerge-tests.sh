#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
KUBECONFIG_FILE="${KUBECONFIG_FILE:-.kubeconfig}"
TILT_LOG_FILE="${TILT_LOG_FILE:-tilt.log}"
TIMEOUT_SECONDS="${TIMEOUT_SECONDS:-300}"
TILT_CI_TIMEOUT="${TILT_CI_TIMEOUT:-45m}"
DOTNET_TEST_LOGGER="${DOTNET_TEST_LOGGER:-html}"

cd "${ROOT_DIR}"

dotnet tool restore
dotnet paket restore
dotnet restore

START_TILT=false ./scripts/start-k3d-tilt.sh

for _ in {1..90}; do
  if KUBECONFIG="${KUBECONFIG_FILE}" kubectl wait \
    --for=condition=Established \
    crd/ingressroutes.traefik.io \
    --timeout=2s >/dev/null 2>&1; then
    break
  fi
  sleep 2
done

KUBECONFIG="${KUBECONFIG_FILE}" kubectl wait \
  --for=condition=Established \
  crd/ingressroutes.traefik.io \
  --timeout=2s

KUBECONFIG="${KUBECONFIG_FILE}" kubectl apply -f demo/k8s/namespace.yaml
KUBECONFIG="${KUBECONFIG_FILE}" kubectl apply -f demo/k8s/secrets.yaml
KUBECONFIG="${KUBECONFIG_FILE}" kubectl apply -f demo/k8s/otel-collector.yaml
KUBECONFIG="${KUBECONFIG_FILE}" kubectl apply -f demo/k8s/ingress.yaml

KUBECONFIG="${KUBECONFIG_FILE}" tilt ci --timeout="${TILT_CI_TIMEOUT}" 2>&1 | tee "${TILT_LOG_FILE}"

TIMEOUT_SECONDS="${TIMEOUT_SECONDS}" ./scripts/wait-for-tilt-api-integration-tests.sh

test_projects=(
  "test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj"
  "test/OrganisationRegistry.KboMutations.UnitTests/OrganisationRegistry.KboMutations.UnitTests.csproj"
  "test/OrganisationRegistry.SqlServer.IntegrationTests/OrganisationRegistry.SqlServer.IntegrationTests.csproj"
  "test/OrganisationRegistry.ElasticSearch.Tests/OrganisationRegistry.ElasticSearch.Tests.csproj"
  "test/OrganisationRegistry.Api.IntegrationTests/OrganisationRegistry.Api.IntegrationTests.csproj"
)

for test_project in "${test_projects[@]}"; do
  ElasticSearch__ReadConnectionString="${ElasticSearch__ReadConnectionString:-http://opensearch.localhost:9080/}" \
  ElasticSearch__WriteConnectionString="${ElasticSearch__WriteConnectionString:-http://opensearch.localhost:9080/}" \
  dotnet test "${test_project}" --no-restore --logger "${DOTNET_TEST_LOGGER}"
done
