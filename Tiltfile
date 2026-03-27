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

k8s_yaml('demo/k8s/namespace.yaml')
k8s_yaml('demo/k8s/secrets.yaml')

# =============================================================================
# Keycloak realm ConfigMap — built from keycloak/realm-export.json
# =============================================================================

local_resource(
    'keycloak-realm-configmap',
    'KUBECONFIG=.kubeconfig kubectl create configmap keycloak-realm --from-file=keycloak/realm-export.json -n wegwijs-demo --dry-run=client -o yaml | KUBECONFIG=.kubeconfig kubectl apply -f -',
    deps=['keycloak/realm-export.json'],
    labels=['setup'],
    resource_deps=['namespace'],
)

# Pseudo-resource to track namespace creation
local_resource(
    'namespace',
    'KUBECONFIG=.kubeconfig kubectl wait --for=jsonpath={.status.phase}=Active namespace/wegwijs-demo --timeout=60s',
    labels=['setup'],
)

# =============================================================================
# Infrastructure
# =============================================================================

k8s_yaml('demo/k8s/mssql.yaml')
k8s_yaml('demo/k8s/keycloak.yaml')

k8s_resource('mssql',
    port_forwards='11433:1433',
    labels=['infrastructure'],
    resource_deps=['namespace'])

# =============================================================================
# Application Images — custom_build pushes to local k3d registry
# =============================================================================

# API — build context is repo root
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-api:local',
    'docker build -t $EXPECTED_REF -f api/Dockerfile . && docker push $EXPECTED_REF',
    deps=[
        'api/Dockerfile',
        'src/OrganisationRegistry',
        'src/OrganisationRegistry.Api',
        'src/OrganisationRegistry.SqlServer',
        'src/OrganisationRegistry.Infrastructure',
        'paket.dependencies',
        'paket.lock',
    ],
    ignore=['**/bin', '**/obj'],
)

# UI — Angular frontend (pre-built in wwwroot)
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-ui:local',
    'docker build -t $EXPECTED_REF src/OrganisationRegistry.UI && docker push $EXPECTED_REF',
    deps=[
        'src/OrganisationRegistry.UI/Dockerfile',
        'src/OrganisationRegistry.UI/wwwroot',
        'src/OrganisationRegistry.UI/default.conf',
        'src/OrganisationRegistry.UI/init.sh',
        'src/OrganisationRegistry.UI/config.js',
    ],
)

# Seed — Python script
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-seed:local',
    'docker build -t $EXPECTED_REF seed && docker push $EXPECTED_REF',
    deps=[
        'seed/Dockerfile',
        'seed/seed.py',
    ],
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

# =============================================================================
# Applications
# =============================================================================

k8s_yaml('demo/k8s/api.yaml')
k8s_yaml('demo/k8s/ui.yaml')
k8s_yaml('demo/k8s/seed.yaml')
k8s_yaml('demo/k8s/m2m.yaml')
k8s_yaml('demo/k8s/nuxt-bff.yaml')
k8s_yaml('demo/k8s/ingress.yaml')

k8s_resource('api',
    labels=['applications'],
    resource_deps=['mssql', 'keycloak'],
    links=[link('http://api.localhost:9080/v1', 'API')])

k8s_resource('ui',
    labels=['applications'],
    resource_deps=['api', 'keycloak'],
    links=[link('http://ui.localhost:9080', 'Angular UI')])

k8s_resource('seed',
    labels=['setup'],
    resource_deps=['api'])

k8s_resource('m2m-demo',
    labels=['demo'],
    resource_deps=['api', 'keycloak'],
    links=[link('http://m2m.localhost:9080', 'M2M Demo')])

k8s_resource('nuxt-bff',
    labels=['demo'],
    resource_deps=['api', 'keycloak'],
    links=[link('http://app.localhost:9080', 'Nuxt BFF')])

k8s_resource('keycloak',
    labels=['infrastructure'],
    resource_deps=['keycloak-realm-configmap'],
    links=[link('http://keycloak.localhost:9080', 'Keycloak')])

# =============================================================================
# Settings
# =============================================================================

update_settings(
    max_parallel_updates=2,
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
print('║  api.localhost:9080       → Organisation Registry API         ║')
print('║  ui.localhost:9080        → Angular UI (backoffice)           ║')
print('║  m2m.localhost:9080       → M2M demo (client credentials)      ║')
print('║  app.localhost:9080       → Nuxt BFF (Keycloak demo)          ║')
print('╠═══════════════════════════════════════════════════════════════╣')
print('║  Demo users: dev / vlimpers / algemeenbeheerder (pw = user)   ║')
print('╚═══════════════════════════════════════════════════════════════╝')
print('')
