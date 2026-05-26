#!/usr/bin/env bash
set -e

python scripts/ci-md2conf.py CHANGELOG.md VBR \
    --user "${CONFLUENCE_USERNAME}" \
    --password "${CONFLUENCE_PASSWORD}" \
    --orgname "vlaamseoverheid" \
    --title "${CONFLUENCE_TITLE}" \
    --ancestor "Changelog" \
    --nogo
