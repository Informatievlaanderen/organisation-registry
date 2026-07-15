#!/usr/bin/env bash
set -e

python scripts/ci-jiraversion.py ${JIRA_PREFIX}-${JIRA_VERSION} ${JIRA_PROJECT} \
    --user "${CONFLUENCE_USERNAME}" \
    --password "${CONFLUENCE_PASSWORD}" \
    --orgname "vlaamseoverheid" \
    --github "https://github.com/Informatievlaanderen" \
    --repo "${CONFLUENCE_TITLE}"
