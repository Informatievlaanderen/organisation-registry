#!/usr/bin/env bash
# tunnel.sh: toggle a Telepresence tunnel (replace/detach) for a workload.
set -euo pipefail

NS="${NAMESPACE:-wegwijs-demo}"
resource="" state=""

usage() {
  cat <<EOF
Usage:
  tunnel.sh --resource NAME              print current state (on|off)
  tunnel.sh --resource NAME --state on   replace NAME with your local process
  tunnel.sh --resource NAME --state off  detach, restore the cluster pod
  tunnel.sh                              this help

Namespace: \$NAMESPACE (default: wegwijs-demo). Needs the telepresence CLI.
EOF
}
die() { echo "tunnel: $*" >&2; exit 1; }

[ $# -eq 0 ] && { usage; exit 0; }
while [ $# -gt 0 ]; do
  case $1 in
    --resource) resource=${2:?--resource needs a value}; shift 2 ;;
    --state)    state=${2:?--state needs on|off}; shift 2 ;;
    -h|--help)  usage; exit 0 ;;
    *)          usage >&2; die "unknown argument: $1" ;;
  esac
done
[ -n "$resource" ] || die "--resource is required"
case $state in ""|on|off) ;; *) die "--state must be on or off" ;; esac

# Always connect explicitly (idempotent); never rely on the deprecated implicit connect.
telepresence connect --namespace "$NS" >&2 ||
  die "not connected. Run 'telepresence connect --namespace $NS' in a terminal (needs sudo once)"

# On = the workload shows up in the replaced list.
# Captured into a variable: `| grep -q` under pipefail can fail on SIGPIPE.
is_on() {
  local out
  out=$(telepresence list --replacements -n "$NS" 2>/dev/null) || return 1
  grep -Eq "(^|[[:space:]])${resource}($|[[:space:]:])" <<<"$out"
}

case $state in
  "")  if is_on; then echo on; else echo off; fi ;;
  on)  if is_on; then echo "$resource: already on" >&2
       else telepresence replace "$resource" >&2; fi ;;
  off) if is_on; then telepresence detach "$resource" >&2
       else echo "$resource: already off" >&2; fi ;;
esac
