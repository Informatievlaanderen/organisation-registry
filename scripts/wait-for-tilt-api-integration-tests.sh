#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
API_BASE="${API_BASE:-http://api.localhost:9080}"
JWT_SIGNING_KEY="${JWT_SIGNING_KEY:-keycloak-demo-local-dev-secret-key-32b}"
JWT_ISSUER="${JWT_ISSUER:-organisatieregister}"
JWT_AUDIENCE="${JWT_AUDIENCE:-organisatieregister}"
JWT_SUBJECT="${JWT_SUBJECT:-api-integration-tests}"
JWT_ACM_ID="${JWT_ACM_ID:-A5C5BFCD-266C-4CC7-9869-4B95E34C090D}"
TIMEOUT_SECONDS="${TIMEOUT_SECONDS:-180}"
POLL_INTERVAL_SECONDS="${POLL_INTERVAL_SECONDS:-2}"

mint_jwt() {
  JWT_SIGNING_KEY="$JWT_SIGNING_KEY" \
  JWT_ISSUER="$JWT_ISSUER" \
  JWT_AUDIENCE="$JWT_AUDIENCE" \
  JWT_SUBJECT="$JWT_SUBJECT" \
  JWT_ACM_ID="$JWT_ACM_ID" \
  python3 - <<'PY'
import base64
import hashlib
import hmac
import json
import os
import time

def b64url(raw: bytes) -> str:
    return base64.urlsafe_b64encode(raw).rstrip(b"=").decode("ascii")

now = int(time.time())
header = {"alg": "HS256", "typ": "JWT"}
payload = {
    "iss": os.environ["JWT_ISSUER"],
    "aud": os.environ["JWT_AUDIENCE"],
    "sub": os.environ["JWT_SUBJECT"],
    "nbf": now,
    "iat": now,
    "exp": now + 60 * 60,
    "vo_id": os.environ["JWT_ACM_ID"],
    "given_name": "Algemeenbeheerder",
    "family_name": "Persona",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "algemeenBeheerder",
}

header_segment = b64url(json.dumps(header, separators=(",", ":")).encode("utf-8"))
payload_segment = b64url(json.dumps(payload, separators=(",", ":")).encode("utf-8"))
signing_input = f"{header_segment}.{payload_segment}"
signature = hmac.new(
    os.environ["JWT_SIGNING_KEY"].encode("utf-8"),
    signing_input.encode("utf-8"),
    hashlib.sha256,
).digest()
print(f"{signing_input}.{b64url(signature)}")
PY
}

AUTH_TOKEN="$(mint_jwt)"

json_get() {
  curl -fsS \
    -H "Accept: application/json" \
    -H "Authorization: Bearer ${AUTH_TOKEN}" \
    "${API_BASE}$1"
}

import_ready() {
  API_BASE="$API_BASE" AUTH_TOKEN="$AUTH_TOKEN" python3 - <<'PY'
import json
import os
import sys
import urllib.request

api_base = os.environ["API_BASE"].rstrip("/")
token = os.environ["AUTH_TOKEN"]
headers = {
    "Accept": "application/json",
    "Authorization": f"Bearer {token}",
}

def get_json(path: str):
    request = urllib.request.Request(f"{api_base}{path}", headers=headers)
    with urllib.request.urlopen(request, timeout=15) as response:
        return json.load(response)

for route in ("/v1/people", "/v1/buildings", "/v1/functiontypes", "/v1/capacities"):
    if not get_json(route):
        sys.exit(1)

parent_organisation_id = "4e83f3ff-4154-4719-833c-d1a8c77568c0"
child_organisation_id = "24fe3a2f-f5d0-4895-acac-3b1918ca1ec7"

if not get_json(f"/v1/organisations/{parent_organisation_id}"):
    sys.exit(1)
if not get_json(f"/v1/organisations/{child_organisation_id}"):
    sys.exit(1)
if not get_json(f"/v1/organisations/{parent_organisation_id}/contacts"):
    sys.exit(1)
if not get_json(f"/v1/organisations/{parent_organisation_id}/children"):
    sys.exit(1)

sys.exit(0)
PY
}

wait_for_status() {
  local deadline=$((SECONDS + TIMEOUT_SECONDS))

  until curl -fsS "${API_BASE}/v1/status" >/dev/null 2>&1; do
    if (( SECONDS >= deadline )); then
      echo "Timed out waiting for ${API_BASE}/v1/status" >&2
      return 1
    fi

    sleep "${POLL_INTERVAL_SECONDS}"
  done
}

wait_until_ready() {
  local deadline=$((SECONDS + TIMEOUT_SECONDS))

  until import_ready; do
    if (( SECONDS >= deadline )); then
      echo "Timed out waiting for imported Tilt data on ${API_BASE}" >&2
      return 1
    fi

    sleep "${POLL_INTERVAL_SECONDS}"
  done
}

echo "Waiting for Tilt API on ${API_BASE}..."
wait_for_status

if ! import_ready; then
  echo "Imported relation data ontbreekt; Piavo-import wordt gestart..."
  (
    cd "${ROOT_DIR}/test/OrganisationRegistry.Api.IntegrationTests"
    dotnet run \
      --project ../OrganisationRegistry.Import.Piavo/OrganisationRegistry.Import.Piavo.csproj \
      -- \
      "${API_BASE}" \
      "${AUTH_TOKEN}"
  )
fi

echo "Waiting for imported data and projections..."
wait_until_ready

echo "Tilt API integration test readiness is OK."
