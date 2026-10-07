# Tiltfile for Wegwijs / Organisation Registry Development
# Run with: tilt up (from repo root)
#
# Prerequisites:
#   k3d cluster:   k3d cluster create --config k3d.config.yaml
#   Traefik:       helm upgrade --install traefik traefik/traefik -f demo/helm/traefik-values.yaml -n traefik --create-namespace

allow_k8s_contexts('k3d-wegwijs-dev')

# =============================================================================
# Namespace & Secrets
# =============================================================================

k8s_yaml([
  'demo/k8s/namespace.yaml',
  'demo/k8s/secrets.yaml',
  'demo/k8s/mssql.yaml',
  'demo/k8s/opensearch.yaml',
  'demo/k8s/keycloak.yaml',
  'demo/k8s/wiremock.yaml',
  'demo/k8s/seq.yaml',
  'demo/k8s/otel-collector.yaml'
])

# =============================================================================
# Keycloak realm ConfigMap — built from keycloak/realm-export.json
# =============================================================================

local_resource(
    'keycloak-realm-configmap',
    'KUBECONFIG=.kubeconfig kubectl create configmap keycloak-realm --from-file=keycloak/realm-export.json -n wegwijs-demo --dry-run=client -o yaml | KUBECONFIG=.kubeconfig kubectl apply -f -',
    deps=['keycloak/realm-export.json'],
    labels=['setup'],
    resource_deps=['namespace'],
    allow_parallel=True
)

local_resource(
    'wiremock-mappings-configmap',
    'KUBECONFIG=.kubeconfig kubectl create configmap wiremock-mappings --from-file=wiremock/mappings -n wegwijs-demo --dry-run=client -o yaml | KUBECONFIG=.kubeconfig kubectl apply -f -',
    deps=['wiremock/mappings'],
    labels=['setup'],
    resource_deps=['namespace'],
    allow_parallel=True
)

local_resource(
    'wiremock-files-configmap',
    'KUBECONFIG=.kubeconfig kubectl create configmap wiremock-files --from-file=wiremock/files -n wegwijs-demo --dry-run=client -o yaml | KUBECONFIG=.kubeconfig kubectl apply -f -',
    deps=['wiremock/files'],
    labels=['setup'],
    resource_deps=['namespace'],
    allow_parallel=True
)

# Refresh kubeconfig from k3d — certs are regenerated on cluster create
local_resource(
    'kubeconfig',
    'k3d kubeconfig get wegwijs-dev > .kubeconfig',
    labels=['setup'],
    allow_parallel=True
)

# Pseudo-resource to track namespace creation
local_resource(
    'namespace',
    'KUBECONFIG=.kubeconfig kubectl apply -f demo/k8s/namespace.yaml && KUBECONFIG=.kubeconfig kubectl wait --for=jsonpath={.status.phase}=Active namespace/wegwijs-demo --timeout=60s',
    labels=['setup'],
    resource_deps=['kubeconfig'],
    allow_parallel=True
)

local_resource(
    'clear-database',
    './scripts/clear-database.sh',
    deps=['scripts/clear-database.sh'],
    labels=['setup'],
    resource_deps=['mssql', 'opensearch'],
    allow_parallel=True
)

local_resource(
    'api-configuration',
    './scripts/seed-tilt-api-configuration.sh',
    deps=['scripts/seed-tilt-api-configuration.sh'],
    labels=['setup'],
    resource_deps=['api'],
    allow_parallel=True
)

# =============================================================================
# Infrastructure
# =============================================================================

k8s_resource('mssql',
    port_forwards='21433:1433',
    labels=['infrastructure'],
    resource_deps=['namespace'])

k8s_resource('opensearch',
    labels=['infrastructure'],
    resource_deps=['namespace'],
    links=[link('http://opensearch.localhost:9080', 'OpenSearch')])

k8s_resource('wiremock',
    port_forwards='8080:8080',
    labels=['infrastructure'],
    resource_deps=['wiremock-mappings-configmap', 'wiremock-files-configmap'],
    links=[link('http://mock.localhost:9080', 'WireMock')],
    pod_readiness='ignore')

k8s_resource('seq',
    labels=['infrastructure'],
    resource_deps=['namespace'],
    links=[link('http://seq.localhost:9080', 'Seq')])

k8s_resource('otel-collector',
    labels=['infrastructure'],
    resource_deps=['seq'],
    pod_readiness='wait')

# =============================================================================
# Application Images — build and push to k3d registry
# =============================================================================

# API — build context is repo root
docker_build(
    'k3d-wegwijs-registry:5051/wegwijs-api:local',
    '.',
    dockerfile='api/Dockerfile',
    only=[
        'api/Dockerfile',
        '.config/dotnet-tools.json',
        'SolutionInfo.cs',
        'organisationregistry-api.pfx',
        'Directory.Build.props',
        'Directory.Packages.props',
        'global.json',
        'OrganisationRegistry.sln',
        'src/OrganisationRegistry',
        'src/OrganisationRegistry.Api',
        'src/OrganisationRegistry.Configuration.Database',
        'src/OrganisationRegistry.ElasticSearch',
        'src/OrganisationRegistry.Infrastructure',
        'src/OrganisationRegistry.Magda',
        'src/OrganisationRegistry.OpenTelemetry',
        'src/OrganisationRegistry.SqlServer',
        'src/Osc',
        'src/OpenSearch.Net',
    ],
    ignore=['**/bin', '**/obj'],
)

# UI — Angular frontend matching exact GitHub Actions CI process
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-ui:local',
    'DOCKER_BUILDKIT=1 docker build -f src/OrganisationRegistry.UI/Dockerfile.optimized -t $EXPECTED_REF . && docker tag $EXPECTED_REF localhost:5051/wegwijs-ui:local && docker push localhost:5051/wegwijs-ui:local',
    deps=[
        'src/OrganisationRegistry.UI/Dockerfile.optimized',
        'src/OrganisationRegistry.UI/app',
        'src/OrganisationRegistry.UI/assets',
        'src/OrganisationRegistry.UI/Infrastructure',
        'src/OrganisationRegistry.UI/default.conf',
        'src/OrganisationRegistry.UI/init.sh',
        'src/OrganisationRegistry.UI/config.js',
        'src/OrganisationRegistry.UI/index.html',
        'src/OrganisationRegistry.UI/main.aot.ts',
        'src/OrganisationRegistry.UI/main.browser.ts',
        'src/OrganisationRegistry.UI/polyfills.browser.ts',
        'src/OrganisationRegistry.UI/vendor.browser.ts',
        'src/OrganisationRegistry.UI/custom-typings.d.ts',
        'package.json',
        'package-lock.json',
        'tsconfig.aot.json',
        'config/',
        'scripts/',
        'organisationregistry-ui.pfx',
    ],
    ignore=['**/node_modules', '**/bin', '**/obj', '**/*.map', 'src/OrganisationRegistry.UI/wwwroot', 'src/OrganisationRegistry.UI/dist'],
)

# PIAVO Import — .NET application
docker_build(
    'k3d-wegwijs-registry:5051/wegwijs-piavo-import:local',
    '.',
    dockerfile='test/OrganisationRegistry.Import.Piavo/Dockerfile',
    only=[
        'test/OrganisationRegistry.Import.Piavo/Dockerfile',
        '.config/dotnet-tools.json',
        'organisationregistry-api.pfx',
        'organisationregistry-ui.pfx',
        'SolutionInfo.cs',
        'Directory.Build.props',
        'Directory.Packages.props',
        'global.json',
        'OrganisationRegistry.sln',
        'src/OrganisationRegistry',
        'src/OrganisationRegistry.Api',
        'src/OrganisationRegistry.Configuration.Database',
        'src/OrganisationRegistry.ElasticSearch',
        'src/OrganisationRegistry.Infrastructure',
        'src/OrganisationRegistry.Magda',
        'src/OrganisationRegistry.OpenTelemetry',
        'src/OrganisationRegistry.SqlServer',
        'src/Osc',
        'src/OpenSearch.Net',
        'test/OrganisationRegistry.Import.Piavo/',
    ],
    ignore=['**/bin', '**/obj'],
)

# M2M demo
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-m2m:local',
    'docker build -t $EXPECTED_REF demo/m2m && docker push $EXPECTED_REF',
    deps=['demo/m2m/'],
)

# Nuxt BFF
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-nuxt-bff:local',
    'docker build -t $EXPECTED_REF demo/nuxt-bff && docker push $EXPECTED_REF',
    deps=['demo/nuxt-bff/'],
)

# Seed — populates required parameter/reference data (KeyTypes, LabelTypes,
# LifecyclePhaseTypes, ...) via the API. Idempotent, safe to re-run.
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-seed:local',
    'docker build -t $EXPECTED_REF demos/seed && docker push $EXPECTED_REF',
    deps=['demos/seed/'],
)

# =============================================================================
# Applications
# =============================================================================

k8s_yaml('demo/k8s/api.yaml')
k8s_yaml('demo/k8s/ui.yaml')
k8s_yaml('demo/k8s/piavo-import.yaml')
k8s_yaml('demo/k8s/m2m.yaml')
k8s_yaml('demo/k8s/nuxt-bff.yaml')
k8s_yaml('demo/k8s/ingress.yaml')
k8s_yaml('demo/k8s/seed.yaml')

# Group all Traefik IngressRoutes into a single Tilt resource so they are
# always applied on `tilt up`, survive `tilt down`/re-up cycles, and are
# visible/manageable in the Tilt UI. Without this, the IngressRoute objects
# are loaded silently and can appear to "disappear" after cluster restarts.
local_resource(
    'wait-traefik-crds',
    cmd='''
    for i in $(seq 1 90); do
      kubectl get crd ingressroutes.traefik.io >/dev/null 2>&1 && break
      sleep 2
    done
    kubectl wait --for=condition=Established crd/ingressroutes.traefik.io --timeout=60s 2>/dev/null
    ''',
    labels=['infrastructure'],
)
k8s_resource(
    objects=[
        'wegwijs-demo:ingressroute',
        'wegwijs-demo-tls:ingressroute',
        'https-to-http:middleware',
    ],
    new_name='ingress-routes',
    resource_deps=['wait-traefik-crds'],
    labels=['infrastructure'],
    pod_readiness='ignore'
)

k8s_resource('api',
    labels=['apps'],
    resource_deps=['clear-database', 'mssql', 'opensearch', 'keycloak', 'wiremock', 'otel-collector'],
    links=[link('http://api.localhost:9080/v1', 'API')],
    trigger_mode=TRIGGER_MODE_MANUAL)

k8s_resource('ui',
    labels=['apps'],
    links=[link('http://ui.localhost:9080', 'UI')])

# piavo-import must run after 'seed': both create overlapping master data
# (KeyTypes, LabelTypes, ContactTypes, LocationTypes, ClassificationTypes,
# FormalFrameworks, Capacities, Purposes) via the API. Running them
# concurrently races the same POSTs against the API. Sequencing after 'seed'
# removes the race entirely. The import is fully idempotent — every
# create-or-skip step checks existence first, so re-running on every tilt up
# is safe and avoids stale "already completed" checks.
k8s_resource('piavo-import',
    labels=['setup'],
    resource_deps=['api-configuration', 'seed'],
    auto_init=True,
    trigger_mode=TRIGGER_MODE_MANUAL)

k8s_resource('m2m-demo',
    labels=['apps'],
    links=[link('http://m2m.localhost:9080', 'M2M Demo')],
    auto_init=True)

k8s_resource('nuxt-bff',
    labels=['apps'],
    links=[link('http://app.localhost:9080', 'Nuxt BFF')],
    auto_init=True)

k8s_resource('keycloak',
    labels=['infrastructure'],
    resource_deps=['keycloak-realm-configmap'],
    links=[link('http://keycloak.localhost:9080', 'Keycloak')])

k8s_resource('seed',
    labels=['setup'],
    resource_deps=['api-configuration', 'keycloak'])

# =============================================================================
# Telepresence — run the API locally (IDE/debugger) inside the cluster network
# =============================================================================
local_resource(
    'telepresence',
    'KUBECONFIG=.kubeconfig telepresence helm install || KUBECONFIG=.kubeconfig telepresence helm upgrade',
    labels=['setup'],
    resource_deps=['kubeconfig'],
)

load('ext://uibutton', 'cmd_button', 'bool_input', 'choice_input')

cmd_button('api:connect',
    argv=['./scripts/tunnel.sh', '--resource', 'api', '--state', 'on'],
    resource='api', icon_name='verified_user', text='Tunnel connect')

cmd_button('api:disconnect',
    argv=['./scripts/tunnel.sh', '--resource', 'api', '--state', 'off'],
    resource='api', icon_name='close', text='Tunnel disconnect')

# =============================================================================
# Settings
# =============================================================================

update_settings(
    max_parallel_updates=3,
    k8s_upsert_timeout_secs=300,
)

# =============================================================================
# Info
# =============================================================================

print('')
print('╔═══════════════════════════════════════════════════════════════╗')
print('║  Wegwijs / Organisation Registry - Development Environment    ║')
print('╠═══════════════════════════════════════════════════════════════╣')
print('║  keycloak.localhost:9080  → Keycloak (admin/admin)            ║')
print('║  seq.localhost:9080       → Seq (structured logs / OTLP)      ║')
print('║  opensearch.localhost:9080 → OpenSearch                       ║')
print('║  mock.localhost:9080      → WireMock (MAGDA mock)             ║')
print('║  api.localhost:9080       → Organisation Registry API         ║')
print('║  ui.localhost:9080        → Angular UI (backoffice)           ║')
print('║  m2m.localhost:9080       → M2M demo (client credentials)     ║')
print('║  app.localhost:9080       → Nuxt BFF (Keycloak demo)          ║')
print('╠═══════════════════════════════════════════════════════════════╣')
print('║  Demo users: dev / vlimpers / algemeenbeheerder (pw = user)   ║')
print('╚═══════════════════════════════════════════════════════════════╝')
print('')
