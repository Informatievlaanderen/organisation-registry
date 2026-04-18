#!/usr/bin/env bash
# =============================================================================
# dev-setup.sh — Wegwijs demo environment opzetten vanaf een verse repo clone
#
# Wat dit script doet:
#   1. Vereisten checken (k3d, kubectl, helm, tilt, docker)
#   2. k3d cluster aanmaken (inclusief registry op localhost:5051)
#   3. .kubeconfig schrijven voor Tilt (Tilt gebruikt KUBECONFIG=.kubeconfig)
#   4. Traefik installeren als ingress controller
#   5. /etc/hosts entry toevoegen voor *.localhost → 127.0.0.1 (indien nodig)
#   6. tilt up starten
#
# Gebruik:
#   bash scripts/dev-setup.sh          # volledig opzetten + tilt up
#   bash scripts/dev-setup.sh --check  # alleen vereisten checken
#   bash scripts/dev-setup.sh --reset  # cluster verwijderen en opnieuw aanmaken
#
# Vereisten (niet automatisch geïnstalleerd):
#   - docker
#   - k3d  (https://k3d.io)
#   - kubectl
#   - helm
#   - tilt (https://tilt.dev)
# =============================================================================

set -euo pipefail

CLUSTER_NAME="wegwijs-dev"
KUBECONFIG_FILE=".kubeconfig"
TRAEFIK_NAMESPACE="traefik"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Kleuren
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

info()    { echo -e "${BLUE}[info]${NC}  $*"; }
ok()      { echo -e "${GREEN}[ok]${NC}    $*"; }
warn()    { echo -e "${YELLOW}[warn]${NC}  $*"; }
error()   { echo -e "${RED}[error]${NC} $*" >&2; }
die()     { error "$*"; exit 1; }

# =============================================================================
# Vereisten checken
# =============================================================================

check_requirements() {
    info "Vereisten checken..."
    local missing=0

    for cmd in docker k3d kubectl helm tilt; do
        if command -v "$cmd" &>/dev/null; then
            ok "$cmd gevonden ($(command -v $cmd))"
        else
            error "$cmd niet gevonden — installeer het eerst"
            missing=$((missing + 1))
        fi
    done

    if ! docker info &>/dev/null; then
        die "Docker daemon niet bereikbaar. Start Docker eerst."
    fi

    if [[ $missing -gt 0 ]]; then
        die "$missing vereiste(n) ontbreken."
    fi

    ok "Alle vereisten aanwezig."
}

# =============================================================================
# k3d cluster aanmaken
# =============================================================================

setup_cluster() {
    cd "$REPO_ROOT"

    if k3d cluster list 2>/dev/null | grep -q "^$CLUSTER_NAME\b"; then
        ok "k3d cluster '$CLUSTER_NAME' bestaat al."
    else
        info "k3d cluster '$CLUSTER_NAME' aanmaken..."
        k3d cluster create --config k3d.config.yaml
        ok "Cluster aangemaakt."
    fi

    # .kubeconfig schrijven voor Tilt (apart van ~/.kube/config)
    info ".kubeconfig schrijven voor Tilt..."
    k3d kubeconfig get "$CLUSTER_NAME" > "$KUBECONFIG_FILE"
    ok ".kubeconfig geschreven."

    # Context ook in ~/.kube/config zetten zodat Tilt de allow_k8s_contexts check doorstaat
    info "k3d context mergen in ~/.kube/config..."
    k3d kubeconfig merge "$CLUSTER_NAME" --kubeconfig-merge-default 2>/dev/null || true
    ok "Context 'k3d-$CLUSTER_NAME' staat in ~/.kube/config."
}

# =============================================================================
# Traefik installeren
# =============================================================================

setup_traefik() {
    export KUBECONFIG="$REPO_ROOT/$KUBECONFIG_FILE"

    if helm status traefik -n "$TRAEFIK_NAMESPACE" &>/dev/null; then
        ok "Traefik al geïnstalleerd."
    else
        info "Traefik helm repo toevoegen..."
        helm repo add traefik https://traefik.github.io/charts 2>/dev/null || true
        helm repo update traefik

        info "Traefik installeren..."
        helm upgrade --install traefik traefik/traefik \
            -f "$REPO_ROOT/demo/helm/traefik-values.yaml" \
            -n "$TRAEFIK_NAMESPACE" \
            --create-namespace \
            --wait \
            --timeout 120s
        ok "Traefik geïnstalleerd."
    fi
}

# =============================================================================
# /etc/hosts — *.localhost entries
# Normaal gesproken lost de OS *.localhost al op naar 127.0.0.1, maar op
# sommige Linux systemen werkt dit niet. Voeg entries toe als dat het geval is.
# =============================================================================

check_hosts() {
    if curl -s --max-time 2 'http://api.localhost:9080' &>/dev/null; then
        ok "*.localhost resolvet correct naar 127.0.0.1."
        return
    fi

    warn "*.localhost resolvet mogelijk niet. Controleer /etc/hosts."
    warn "Voeg deze regels toe aan /etc/hosts als de services niet bereikbaar zijn:"
    echo ""
    echo "    127.0.0.1  api.localhost"
    echo "    127.0.0.1  ui.localhost"
    echo "    127.0.0.1  app.localhost"
    echo "    127.0.0.1  keycloak.localhost"
    echo "    127.0.0.1  seq.localhost"
    echo "    127.0.0.1  opensearch.localhost"
    echo "    127.0.0.1  mock.localhost"
    echo "    127.0.0.1  m2m.localhost"
    echo ""
}

# =============================================================================
# Cluster verwijderen (--reset)
# =============================================================================

reset_cluster() {
    warn "Cluster '$CLUSTER_NAME' verwijderen..."
    k3d cluster delete "$CLUSTER_NAME" 2>/dev/null || true
    rm -f "$REPO_ROOT/$KUBECONFIG_FILE"
    ok "Cluster verwijderd. Draai het script opnieuw om een nieuw cluster aan te maken."
    exit 0
}

# =============================================================================
# Hoofdprogramma
# =============================================================================

main() {
    cd "$REPO_ROOT"

    case "${1:-}" in
        --check)
            check_requirements
            exit 0
            ;;
        --reset)
            check_requirements
            reset_cluster
            ;;
        "")
            ;;
        *)
            die "Onbekende optie: $1\nGebruik: $0 [--check|--reset]"
            ;;
    esac

    echo ""
    echo "╔═══════════════════════════════════════════════════════════════╗"
    echo "║  Wegwijs / Organisation Registry — Dev Setup                  ║"
    echo "╚═══════════════════════════════════════════════════════════════╝"
    echo ""

    check_requirements
    echo ""

    setup_cluster
    echo ""

    setup_traefik
    echo ""

    check_hosts
    echo ""

    ok "Setup klaar. Tilt starten..."
    echo ""
    echo "  Services worden beschikbaar op:"
    echo "    http://keycloak.localhost:9080  — Keycloak (admin / admin)"
    echo "    http://seq.localhost:9080       — Seq (logs)"
    echo "    http://opensearch.localhost:9080 — OpenSearch"
    echo "    http://mock.localhost:9080      — WireMock"
    echo "    http://api.localhost:9080/v1    — Organisation Registry API"
    echo "    http://ui.localhost:9080        — Angular UI"
    echo "    http://app.localhost:9080       — Nuxt BFF demo"
    echo "    http://m2m.localhost:9080       — M2M demo"
    echo ""
    echo "  Stop Tilt met Ctrl+C"
    echo ""

    KUBECONFIG="$REPO_ROOT/$KUBECONFIG_FILE" tilt up
}

main "$@"
