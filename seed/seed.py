#!/usr/bin/env python3
"""
Local dev seed script voor Organisation Registry.
Mint een Developer JWT en maakt alle benodigde parameter types aan via de REST API.
Idempotent: bestaande items (409 Conflict) worden genegeerd.
"""

import base64
import hashlib
import hmac
import json
import sys
import time
import urllib.request
import urllib.error

# ---------------------------------------------------------------------------
# Config — overschrijfbaar via env vars
# ---------------------------------------------------------------------------
import os

API_BASE = os.environ.get("API_BASE", "http://host.docker.internal:9002")
SIGNING_KEY = os.environ.get(
    "JWT_SIGNING_KEY", "keycloak-demo-local-dev-secret-key-32b"
)
ISSUER = os.environ.get("JWT_ISSUER", "organisatieregister")
AUDIENCE = os.environ.get("JWT_AUDIENCE", "organisatieregister")
# vo_id van de dev user — moet overeenkomen met OIDCAuth:Developers in de DB
DEVELOPER_VO_ID = os.environ.get(
    "DEVELOPER_VO_ID", "9c2f7372-7112-49dc-9771-f127b048b4c7"
)

# ---------------------------------------------------------------------------
# Minimale HS256 JWT implementatie (geen externe deps nodig)
# ---------------------------------------------------------------------------


def _b64url(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode()


def mint_jwt() -> str:
    header = _b64url(json.dumps({"alg": "HS256", "typ": "JWT"}).encode())
    now = int(time.time())
    # Dit is een custom JWT in het formaat dat de API zelf aanmaakt na /v1/security/exchange.
    # De API valideert op issuer, audience en signing key, en leest ClaimTypes.Role.
    # ClaimTypes.Role = http://schemas.microsoft.com/ws/2008/06/identity/claims/role
    # RoleMapping.Map(Role.AlgemeenBeheerder) = "algemeenBeheerder"
    payload = _b64url(
        json.dumps(
            {
                "iss": ISSUER,
                "aud": AUDIENCE,
                "sub": DEVELOPER_VO_ID,
                "iat": now,
                "exp": now + 3600,
                # ClaimTypes.Role — waarde zoals RoleMapping ze aanmaakt
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "algemeenBeheerder",
                # Extra claims voor herkenbaarheid in logs
                "given_name": "Seed",
                "family_name": "Script",
            }
        ).encode()
    )
    msg = f"{header}.{payload}".encode()
    sig = hmac.new(SIGNING_KEY.encode(), msg, hashlib.sha256).digest()
    return f"{header}.{payload}.{_b64url(sig)}"


# ---------------------------------------------------------------------------
# HTTP helper
# ---------------------------------------------------------------------------


def post(path: str, body: dict, token: str) -> tuple[int, dict]:
    url = f"{API_BASE}{path}"
    data = json.dumps(body).encode()
    req = urllib.request.Request(
        url,
        data=data,
        method="POST",
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Bearer {token}",
        },
    )
    try:
        with urllib.request.urlopen(req) as resp:
            return resp.status, {}
    except urllib.error.HTTPError as e:
        return e.code, {}


def wait_for_api(token: str, max_wait: int = 120):
    url = f"{API_BASE}/v1/purposes"
    req = urllib.request.Request(url, headers={"Authorization": f"Bearer {token}"})
    deadline = time.time() + max_wait
    while time.time() < deadline:
        try:
            with urllib.request.urlopen(req, timeout=5) as resp:
                if resp.status < 500:
                    print(f"  API bereikbaar ({resp.status})")
                    return
        except Exception as e:
            pass
        print(f"  Wachten op API op {API_BASE} ...")
        time.sleep(5)
    print("ERROR: API niet bereikbaar na wachten. Seed mislukt.")
    sys.exit(1)


# ---------------------------------------------------------------------------
# Seed data
# GUIDs zijn de bekende lokale dev waarden uit appsettings.development.json
# en de Authorization config.
# ---------------------------------------------------------------------------


def seed(token: str):
    created = 0
    skipped = 0
    errors = 0

    def create(label: str, path: str, body: dict):
        nonlocal created, skipped, errors
        status, _ = post(path, body, token)
        if status in (200, 201, 204):
            print(f"  [OK]      {label}")
            created += 1
        elif status == 409:
            print(f"  [bestaat] {label}")
            skipped += 1
        else:
            print(f"  [FOUT {status}] {label}")
            errors += 1

    # --- KeyTypes ---
    print("\n=== KeyTypes ===")
    create(
        "Vlimpers",
        "/v1/keytypes",
        {"id": "922a46bb-1378-45bd-a61f-b6bbf348a4d5", "name": "Vlimpers"},
    )
    create(
        "Orafin",
        "/v1/keytypes",
        {"id": "1e3611a7-7914-411a-a0c9-84fcd6218e67", "name": "Orafin"},
    )
    create(
        "KBO",
        "/v1/keytypes",
        {"id": "a7e93f01-0001-0000-0000-000000000001", "name": "KBO"},
    )
    create(
        "INR",
        "/v1/keytypes",
        {"id": "a7e93f01-0001-0000-0000-000000000002", "name": "INR"},
    )
    create(
        "Vademecum",
        "/v1/keytypes",
        {"id": "a7e93f01-0001-0000-0000-000000000003", "name": "Vademecum"},
    )
    create(
        "Vlimpers kort",
        "/v1/keytypes",
        {"id": "a7e93f01-0001-0000-0000-000000000004", "name": "Vlimpers kort"},
    )

    # --- LabelTypes ---
    print("\n=== LabelTypes ===")
    create(
        "Afkorting",
        "/v1/labeltypes",
        {"id": "1955aff4-6df9-da43-6f32-880989e7d210", "name": "Afkorting"},
    )
    create(
        "Alternatieve naam",
        "/v1/labeltypes",
        {"id": "b46a2eff-9c78-bbc7-de8f-ce2e7a0f40ce", "name": "Alternatieve naam"},
    )
    create(
        "Franse naam",
        "/v1/labeltypes",
        {"id": "a7e93f02-0002-0000-0000-000000000001", "name": "Franse naam"},
    )
    create(
        "Duitse naam",
        "/v1/labeltypes",
        {"id": "a7e93f02-0002-0000-0000-000000000002", "name": "Duitse naam"},
    )
    create(
        "Engelse naam",
        "/v1/labeltypes",
        {"id": "a7e93f02-0002-0000-0000-000000000003", "name": "Engelse naam"},
    )
    create(
        "Formele benaming KBO",
        "/v1/labeltypes",
        {"id": "a7e93f02-0002-0000-0000-000000000004", "name": "Formele benaming KBO"},
    )

    # --- ContactTypes ---
    print("\n=== ContactTypes ===")
    create(
        "E-mail",
        "/v1/contacttypes",
        {
            "id": "a7e93f03-0003-0000-0000-000000000001",
            "name": "E-mail",
            "regex": r"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            "example": "info@voorbeeld.be",
        },
    )
    create(
        "Telefoon",
        "/v1/contacttypes",
        {
            "id": "a7e93f03-0003-0000-0000-000000000002",
            "name": "Telefoon",
            "regex": r"^[0-9\s\+\-\(\)]+$",
            "example": "+32 2 123 45 67",
        },
    )
    create(
        "GSM",
        "/v1/contacttypes",
        {
            "id": "a7e93f03-0003-0000-0000-000000000003",
            "name": "GSM",
            "regex": r"^[0-9\s\+\-\(\)]+$",
            "example": "+32 470 12 34 56",
        },
    )
    create(
        "Website",
        "/v1/contacttypes",
        {
            "id": "a7e93f03-0003-0000-0000-000000000004",
            "name": "Website",
            "regex": r"^https?://",
            "example": "https://www.voorbeeld.be",
        },
    )

    # --- LocationTypes ---
    print("\n=== LocationTypes ===")
    create(
        "Maatschappelijke zetel",
        "/v1/locationtypes",
        {
            "id": "a7e93f04-0004-0000-0000-000000000001",
            "name": "Maatschappelijke zetel",
        },
    )
    create(
        "Operationeel adres",
        "/v1/locationtypes",
        {"id": "a7e93f04-0004-0000-0000-000000000002", "name": "Operationeel adres"},
    )

    # --- Capacities ---
    print("\n=== Capaciteiten ===")
    create(
        "Voorzitter",
        "/v1/capacities",
        {"id": "f41bc4a3-afc3-5754-c8fa-f9a938a48d88", "name": "Voorzitter"},
    )
    create(
        "Secretaris",
        "/v1/capacities",
        {"id": "dfcd7d89-9720-67dd-d425-686cc0139830", "name": "Secretaris"},
    )
    create(
        "Lid",
        "/v1/capacities",
        {"id": "a7e93f05-0005-0000-0000-000000000001", "name": "Lid"},
    )
    create(
        "Ondervoorzitter",
        "/v1/capacities",
        {"id": "a7e93f05-0005-0000-0000-000000000002", "name": "Ondervoorzitter"},
    )

    # --- OrganisationClassificationTypes ---
    print("\n=== OrganisatieclassificatieTypes ===")
    create(
        "Juridische vorm",
        "/v1/organisationclassificationtypes",
        {"id": "a7e93f06-0006-0000-0000-000000000001", "name": "Juridische vorm"},
    )
    create(
        "Beleidsdomein",
        "/v1/organisationclassificationtypes",
        {"id": "a7e93f06-0006-0000-0000-000000000002", "name": "Beleidsdomein"},
    )
    create(
        "Bestuursniveau",
        "/v1/organisationclassificationtypes",
        {"id": "a7e93f06-0006-0000-0000-000000000003", "name": "Bestuursniveau"},
    )
    create(
        "Categorie",
        "/v1/organisationclassificationtypes",
        {"id": "a7e93f06-0006-0000-0000-000000000004", "name": "Categorie"},
    )
    create(
        "Entiteitsvorm",
        "/v1/organisationclassificationtypes",
        {"id": "a7e93f06-0006-0000-0000-000000000005", "name": "Entiteitsvorm"},
    )
    # Bekend uit appsettings Authorization config:
    create(
        "Regelgeving classificatie",
        "/v1/organisationclassificationtypes",
        {
            "id": "cf2ce6fe-395c-0620-1bab-7152fc4d6f76",
            "name": "Regelgeving classificatie",
        },
    )
    create(
        "CJM classificatie A",
        "/v1/organisationclassificationtypes",
        {"id": "af8e2f7d-c68c-8c15-c48e-61ef43e5a264", "name": "CJM classificatie A"},
    )
    create(
        "CJM classificatie B",
        "/v1/organisationclassificationtypes",
        {"id": "910d6f9e-0427-ea76-c69d-7288107d79f9", "name": "CJM classificatie B"},
    )
    create(
        "CJM classificatie C",
        "/v1/organisationclassificationtypes",
        {"id": "c35407e4-8559-08d4-f461-a8247c993d58", "name": "CJM classificatie C"},
    )
    # Bekend uit migratie AddConfigurationSettingForReport:
    create(
        "Mandaten en vermogensaangifte",
        "/v1/organisationclassificationtypes",
        {
            "id": "94944afb-7261-554c-dac6-a19ad4387359",
            "name": "Mandaten en vermogensaangifte",
        },
    )

    # --- FormalFrameworkCategories (nodig voor FormalFrameworks) ---
    print("\n=== FormalFrameworkCategories ===")
    create(
        "Algemeen",
        "/v1/formalframeworkcategories",
        {"id": "a7e93f07-0007-0000-0000-000000000001", "name": "Algemeen"},
    )
    create(
        "Vlaams",
        "/v1/formalframeworkcategories",
        {"id": "a7e93f07-0007-0000-0000-000000000002", "name": "Vlaams"},
    )
    create(
        "Federaal",
        "/v1/formalframeworkcategories",
        {"id": "a7e93f07-0007-0000-0000-000000000003", "name": "Federaal"},
    )

    # --- FormalFrameworks (bekend uit appsettings Authorization config) ---
    print("\n=== FormalFrameworks ===")
    ff_cat = "a7e93f07-0007-0000-0000-000000000001"  # Algemeen
    # Vlimpers-owned:
    create(
        "Vlimpers FF 1",
        "/v1/formalframeworks",
        {
            "id": "38f97d25-eb84-e4ed-f77f-2bf449d53c47",
            "name": "Vlimpers FF 1",
            "code": "VL-FF-01",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 2",
        "/v1/formalframeworks",
        {
            "id": "bab9487c-d865-ac39-6ac3-0eb108e4780f",
            "name": "Vlimpers FF 2",
            "code": "VL-FF-02",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 3",
        "/v1/formalframeworks",
        {
            "id": "cb43e05c-4003-5d4d-26e0-28696b8570b4",
            "name": "Vlimpers FF 3",
            "code": "VL-FF-03",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 4",
        "/v1/formalframeworks",
        {
            "id": "80618743-625b-4fc4-8dba-380cb859f8ad",
            "name": "Vlimpers FF 4",
            "code": "VL-FF-04",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 5",
        "/v1/formalframeworks",
        {
            "id": "c73f7759-e13d-41e8-a267-d63f11aae099",
            "name": "Vlimpers FF 5",
            "code": "VL-FF-05",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 6",
        "/v1/formalframeworks",
        {
            "id": "e99f0506-d310-1d68-cfe4-2150fcf68e83",
            "name": "Vlimpers FF 6",
            "code": "VL-FF-06",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 7",
        "/v1/formalframeworks",
        {
            "id": "a3d184ea-8520-45f0-9e3b-3c7b4b3883d9",
            "name": "Vlimpers FF 7",
            "code": "VL-FF-07",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 8",
        "/v1/formalframeworks",
        {
            "id": "f2f536c3-9c22-47d4-81d0-b77a3843be9e",
            "name": "Vlimpers FF 8",
            "code": "VL-FF-08",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Vlimpers FF 9",
        "/v1/formalframeworks",
        {
            "id": "59cb11a7-f8a8-45b1-9d6a-38c6960ecc0b",
            "name": "Vlimpers FF 9",
            "code": "VL-FF-09",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    # Regelgeving-owned:
    create(
        "Regelgeving FF 1",
        "/v1/formalframeworks",
        {
            "id": "f03e9a22-8864-49c1-9195-790e4d1fed83",
            "name": "Regelgeving FF 1",
            "code": "RG-FF-01",
            "formalFrameworkCategoryId": ff_cat,
        },
    )
    create(
        "Regelgeving FF 2",
        "/v1/formalframeworks",
        {
            "id": "d51abedb-2c71-5da9-abc8-08a673237964",
            "name": "Regelgeving FF 2",
            "code": "RG-FF-02",
            "formalFrameworkCategoryId": ff_cat,
        },
    )

    # --- Purposes ---
    print("\n=== Purposes ===")
    create(
        "Vlaamse overheid",
        "/v1/purposes",
        {"id": "a7e93f09-0009-0000-0000-000000000001", "name": "Vlaamse overheid"},
    )
    create(
        "Lokale overheid",
        "/v1/purposes",
        {"id": "a7e93f09-0009-0000-0000-000000000002", "name": "Lokale overheid"},
    )

    # --- LifecyclePhaseTypes ---
    print("\n=== LifecyclePhaseTypes ===")
    create(
        "Actief",
        "/v1/lifecyclephasetypes",
        {
            "id": "a7e93f0a-000a-0000-0000-000000000001",
            "name": "Actief",
            "representsActivePhase": True,
            "isDefaultPhase": True,
        },
    )
    create(
        "Inactief",
        "/v1/lifecyclephasetypes",
        {
            "id": "a7e93f0a-000a-0000-0000-000000000002",
            "name": "Inactief",
            "representsActivePhase": False,
            "isDefaultPhase": False,
        },
    )

    # --- SeatTypes ---
    print("\n=== SeatTypes ===")
    create(
        "Effectief",
        "/v1/seattypes",
        {
            "id": "a7e93f0b-000b-0000-0000-000000000001",
            "name": "Effectief",
            "order": 1,
            "isEffective": True,
        },
    )
    create(
        "Plaatsvervangend",
        "/v1/seattypes",
        {
            "id": "a7e93f0b-000b-0000-0000-000000000002",
            "name": "Plaatsvervangend",
            "order": 2,
            "isEffective": False,
        },
    )

    # --- MandateRoleTypes ---
    print("\n=== MandateRoleTypes ===")
    create(
        "Voorzitter",
        "/v1/mandateroletypes",
        {"id": "a7e93f0c-000c-0000-0000-000000000001", "name": "Voorzitter"},
    )
    create(
        "Secretaris",
        "/v1/mandateroletypes",
        {"id": "a7e93f0c-000c-0000-0000-000000000002", "name": "Secretaris"},
    )
    create(
        "Lid",
        "/v1/mandateroletypes",
        {"id": "a7e93f0c-000c-0000-0000-000000000003", "name": "Lid"},
    )

    # --- OrganisationRelationTypes ---
    print("\n=== OrganisatieRelatieTypes ===")
    create(
        "Is onderdeel van",
        "/v1/organisationrelationtypes",
        {
            "id": "a7e93f0d-000d-0000-0000-000000000001",
            "name": "Is onderdeel van",
            "inverseName": "Bevat",
        },
    )
    create(
        "Werkt samen met",
        "/v1/organisationrelationtypes",
        {
            "id": "a7e93f0d-000d-0000-0000-000000000002",
            "name": "Werkt samen met",
            "inverseName": "Werkt samen met",
        },
    )

    # --- Summary ---
    print(f"\n{'=' * 50}")
    print(
        f"Seed voltooid: {created} aangemaakt, {skipped} al aanwezig, {errors} fouten"
    )
    if errors:
        sys.exit(1)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

if __name__ == "__main__":
    print("=== Organisation Registry local dev seed ===")
    print(f"  API: {API_BASE}")

    token = mint_jwt()
    print(f"  JWT gemint voor vo_id={DEVELOPER_VO_ID}")

    print("\nWachten op API...")
    wait_for_api(token)

    seed(token)
