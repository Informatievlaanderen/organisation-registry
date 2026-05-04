#!/usr/bin/env bash

set -euo pipefail

CLUSTER_NAME="${CLUSTER_NAME:-wegwijs-dev}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
KUBECONFIG_FILE="$REPO_ROOT/.kubeconfig"

log() {
    printf '[start-k3d-tilt] %s\n' "$*"
}

require_cmd() {
    command -v "$1" >/dev/null 2>&1 || {
        printf 'Missing required command: %s\n' "$1" >&2
        exit 1
    }
}

cluster_exists() {
    k3d cluster list 2>/dev/null | awk 'NR > 1 { print $1 }' | grep -Fxq "$CLUSTER_NAME"
}

cluster_has_server() {
    k3d cluster list 2>/dev/null | awk -v cluster="$CLUSTER_NAME" '$1 == cluster { split($2, servers, "/"); print servers[1] }' | grep -Eq '^[1-9][0-9]*$'
}

main() {
    require_cmd docker
    require_cmd k3d
    require_cmd tilt

    cd "$REPO_ROOT"

    if cluster_exists; then
        if cluster_has_server; then
            log "Starting existing cluster '$CLUSTER_NAME'..."
            k3d cluster start "$CLUSTER_NAME"
        else
            log "Cluster '$CLUSTER_NAME' exists but has no nodes. Recreating it..."
            k3d cluster delete "$CLUSTER_NAME"
            k3d cluster create --config k3d.config.yaml
        fi
    else
        log "Creating cluster '$CLUSTER_NAME' from k3d.config.yaml..."
        k3d cluster create --config k3d.config.yaml
    fi

    log "Refreshing .kubeconfig..."
    k3d kubeconfig get "$CLUSTER_NAME" > "$KUBECONFIG_FILE"

    log "Merging kubeconfig into ~/.kube/config..."
    k3d kubeconfig merge "$CLUSTER_NAME" --kubeconfig-merge-default 2>/dev/null || true

    if [[ "${START_TILT:-true}" == "false" ]]; then
        log "Skipping Tilt start."
        return
    fi

    log "Starting Tilt..."
    KUBECONFIG="$KUBECONFIG_FILE" tilt up
}

main "$@"
