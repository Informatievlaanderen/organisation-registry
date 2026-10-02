#!/usr/bin/env bash

set -euo pipefail

# start-caches.sh: idempotent pull-through registry caches

cache() { # name host-port upstream-url
  docker start "$1" >/dev/null 2>&1 \
    || docker run -d --restart=always --name "$1" -p "$2:5000" \
         -e REGISTRY_PROXY_REMOTEURL="$3" registry:2 >/dev/null
}

cache cache-dockerhub 5000 https://registry-1.docker.io
cache cache-mcr       5001 https://mcr.microsoft.com
cache cache-quay      5002 https://quay.io
