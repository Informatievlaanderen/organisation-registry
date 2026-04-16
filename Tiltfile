# Organisation Registry — Local Development Environment
#
# Prerequisites:
#   - k3d cluster running (k3d cluster create org-registry)
#   - tilt installed (https://docs.tilt.dev/install.html)
#
# Usage:
#   tilt up

# Load all infrastructure manifests
k8s_yaml([
    'k8s/dev/mssql.yaml',
    'k8s/dev/opensearch.yaml',
    'k8s/dev/acm.yaml',
    'k8s/dev/wiremock.yaml',
    'k8s/dev/otel-collector.yaml',
])

# Build IdentityServer (ACM/IDM) image from local source
docker_build(
    'acm',
    context='src/IdentityServer',
    dockerfile='src/IdentityServer/Dockerfile',
)

# Build WireMock image with bundled mappings and response files
docker_build(
    'org-registry/wiremock',
    context='wiremock',
    dockerfile='wiremock/Dockerfile',
    live_update=[
        sync('wiremock/mappings', '/home/wiremock/mappings'),
        sync('wiremock/files', '/home/wiremock/__files'),
    ],
)

# SQL Server — port 21433 on host (same as docker-compose)
k8s_resource(
    'mssql',
    port_forwards=['21433:1433'],
    labels=['infrastructure'],
)

# OpenSearch — ports 9200 and 9600 on host (same as docker-compose)
k8s_resource(
    'opensearch',
    port_forwards=['9200:9200', '9600:9600'],
    labels=['infrastructure'],
)

# ACM / IdentityServer — port 5050 on host (same as docker-compose)
k8s_resource(
    'acm',
    port_forwards=['5050:80'],
    labels=['infrastructure'],
)

# WireMock — port 8080 on host (same as docker-compose)
k8s_resource(
    'wiremock',
    port_forwards=['8080:8080'],
    labels=['infrastructure'],
)

# OpenTelemetry Collector — port 4317 (OTLP gRPC) on host (same as docker-compose)
k8s_resource(
    'otel-collector',
    port_forwards=['4317:4317'],
    labels=['infrastructure'],
)
