# Tiltfile for Wegwijs / Organisation Registry Keycloak Demo
# Run with: tilt up (from repo root)
#
# Prerequisites:
#   k3d cluster:   k3d cluster create wegwijs-dev -p "80:80@loadbalancer" -p "443:443@loadbalancer" --registry-create k3d-wegwijs-registry:5051
#   Traefik:       helm upgrade --install traefik traefik/traefik -f demo/helm/traefik-values.yaml -n traefik --create-namespace
#   Secrets:       demo/k8s/secrets.yaml — edit to match your local .env values

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
    'kubectl wait --for=jsonpath={.status.phase}=Active namespace/wegwijs-demo --timeout=30s && kubectl create configmap keycloak-realm --from-file=keycloak/realm-export.json -n wegwijs-demo --dry-run=client -o yaml | kubectl apply -f -',
    deps=['keycloak/realm-export.json'],
    labels=['infrastructure'],
)

# =============================================================================
# Infrastructure
# =============================================================================

k8s_yaml('demo/k8s/mssql.yaml')
k8s_yaml('demo/k8s/keycloak.yaml')

k8s_resource('mssql',
    port_forwards='1433:1433',
    labels=['infrastructure'])

k8s_resource('keycloak',
    links=['http://keycloak.localhost/admin'],
    labels=['infrastructure'],
    resource_deps=['keycloak-realm-configmap'])

# =============================================================================
# Application Images — custom_build pushes to local k3d registry
# =============================================================================

# API — build context is repo root (paket, src/, SolutionInfo.cs)
# Note: image is built and pushed to local registry via docker push (Tilt handles push automatically)
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-api:local',
    'docker build -t $EXPECTED_REF -f api/Dockerfile . && docker push $EXPECTED_REF',
    deps=[
        'api/Dockerfile',
        'src',
        'paket.dependencies',
        'paket.lock',
        'SolutionInfo.cs',
    ],
)

# Seed — Python script, build context is seed/
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
    deps=['demo/m2m/Dockerfile', 'demo/m2m/'],
)

# Nuxt BFF
custom_build(
    'k3d-wegwijs-registry:5051/wegwijs-nuxt-bff:local',
    'docker build -t $EXPECTED_REF demo/nuxt-bff && docker push $EXPECTED_REF',
    deps=['demo/nuxt-bff/Dockerfile', 'demo/nuxt-bff/'],
)

# =============================================================================
# Applications
# =============================================================================

k8s_yaml('demo/k8s/api.yaml')
k8s_yaml('demo/k8s/seed.yaml')
k8s_yaml('demo/k8s/m2m.yaml')
k8s_yaml('demo/k8s/nuxt-bff.yaml')
k8s_yaml('demo/k8s/ingress.yaml')

k8s_resource('api',
    links=['http://api.localhost/v1/organisations'],
    labels=['applications'],
    resource_deps=['mssql', 'keycloak'])

k8s_resource('seed',
    labels=['applications'],
    resource_deps=['api'])

k8s_resource('m2m-demo',
    links=['http://m2m.localhost'],
    labels=['applications'],
    resource_deps=['api', 'keycloak'])

k8s_resource('nuxt-bff',
    links=['http://app.localhost'],
    labels=['applications'],
    resource_deps=['api', 'keycloak'])

# =============================================================================
# Settings
# =============================================================================

update_settings(
    max_parallel_updates=1,  # avoid k3d import race conditions
    k8s_upsert_timeout_secs=300
)

print('Wegwijs Keycloak Demo ready')
print('')
print('Services:')
print('  Keycloak admin: http://keycloak.localhost/admin')
print('  App (BFF):      http://app.localhost')
print('  API:            http://api.localhost/v1/organisations')
print('  M2M demo:       http://m2m.localhost')
print('')
print('Users: dev / vlimpers / algemeenbeheerder (password: same as username)')
