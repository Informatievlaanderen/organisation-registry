# Tiltfile for Wegwijs / Organisation Registry Development
# Run with: tilt up (from repo root)
#
# Prerequisites:
#   k3d cluster:   k3d cluster create --config k3d.config.yaml
#   Traefik:       helm upgrade --install traefik traefik/traefik -f demo/helm/traefik-values.yaml -n traefik --create-namespace

# =============================================================================
# Settings
# =============================================================================
config.define_string("cluster")
config.define_string("registry")
cfg = config.parse()

CLUSTER     = cfg.get("cluster", "wegwijs-dev")
REGISTRY      = 'k3d-wegwijs-registry:5051'
HOST_REGISTRY = 'localhost:5051'
IMAGE_TAG   = "local"
CONTEXT     = "k3d-" + CLUSTER
CWD         = os.getcwd()
SCRIPTS_DIR = os.path.join(CWD, "scripts")
K8S_DIR     = os.path.join(CWD, "demo/k8s")
NAMESPACE   = read_yaml(os.path.join(K8S_DIR, "namespace.yaml"))["metadata"]["name"]
KUBECONFIG  = os.path.join(CWD, ".kubeconfig")

def image_ref(name):
    """Short image name; default_registry adds the registry, e.g. wegwijs-api:local"""
    return 'wegwijs-%s:%s' % (name, IMAGE_TAG)

def push_image(name):
    """Expected image name that will be pushed, e.g. localhost:5051/wegwijs-api"""
    return 'localhost:5051/wegwijs-%s' % (name)

def setup_resource(name, cmd, **kwargs):
    """A local_resource in the "setup" group that always uses the k3d kubeconfig."""
    local_resource(
        name,
        cmd,
        env={"KUBECONFIG": KUBECONFIG},
        labels=["setup"],
        allow_parallel=True,
        **kwargs
    )

def configmap(name, source):
    """(Re)create a ConfigMap from a file or directory whenever it changes."""
    setup_resource(
        name + "-configmap",
        "kubectl create configmap %s --from-file=%s -n %s --dry-run=client -o yaml | kubectl apply -f -"
        % (name, source, NAMESPACE),
        deps=[source],
        resource_deps=["namespace"],
    )

# =============================================================================
# Setup
# =============================================================================
allow_k8s_contexts(CONTEXT)

update_settings(
    max_parallel_updates=3,
    k8s_upsert_timeout_secs=300,
)
load("ext://uibutton", "cmd_button", "bool_input", "choice_input")
default_registry(HOST_REGISTRY, host_from_cluster=REGISTRY)

k8s_yaml([
  os.path.join(K8S_DIR, "namespace.yaml"),
  os.path.join(K8S_DIR, "secrets.yaml"),
  os.path.join(K8S_DIR, "ingress.yaml"),

  # os.path.join(K8S_DIR, "infra", "elasticsearchprojections.yaml"),
  os.path.join(K8S_DIR, "infra", "keycloak.yaml"),
  os.path.join(K8S_DIR, "infra", "mssql.yaml"),
  os.path.join(K8S_DIR, "infra", "opensearch.yaml"),
  os.path.join(K8S_DIR, "infra", "otel-collector.yaml"),
  os.path.join(K8S_DIR, "infra", "seq.yaml"),
  os.path.join(K8S_DIR, "infra", "wiremock.yaml"),

  os.path.join(K8S_DIR, "apps", "api.yaml"),
  os.path.join(K8S_DIR, "apps", "ui.yaml"),
  os.path.join(K8S_DIR, "apps", "m2m.yaml"),
  os.path.join(K8S_DIR, "apps", "nuxt-bff.yaml"),

  os.path.join(K8S_DIR, "bootstrap", "piavo-import.yaml"),
  os.path.join(K8S_DIR, "bootstrap", "seed.yaml"),
])

# Dynamic ConfigMaps
configmap("keycloak-realm",    "keycloak/realm-export.json")
configmap("wiremock-mappings", "wiremock/mappings")
configmap("wiremock-files",    "wiremock/files")

# Refresh kubeconfig from k3d — certs are regenerated on cluster create
setup_resource("kubeconfig", "k3d kubeconfig get %s > %s" % (CLUSTER, KUBECONFIG))

# Pseudo-resource to track namespace creation
setup_resource(
    "namespace",
    "kubectl apply -f %s && kubectl wait --for=jsonpath={.status.phase}=Active namespace/%s --timeout=60s"
    % (os.path.join(K8S_DIR, "namespace.yaml"), NAMESPACE),
    deps=[os.path.join(K8S_DIR, "namespace.yaml")],
    resource_deps=["kubeconfig"],
)

setup_resource("clear-database", os.path.join(SCRIPTS_DIR, "clear-database.sh"),resource_deps=["mssql", "opensearch"])
setup_resource("api-configuration", os.path.join(SCRIPTS_DIR, "seed-tilt-api-configuration.sh"),resource_deps=["api"])
setup_resource("wait-traefik-crds",
    cmd="""
    for i in $(seq 1 90); do
      kubectl get crd ingressroutes.traefik.io >/dev/null 2>&1 && break
      sleep 2
    done
    kubectl wait --for=condition=Established crd/ingressroutes.traefik.io --timeout=60s 2>/dev/null
    """,
)

# =============================================================================
# Infra
# =============================================================================

k8s_resource("mssql",
    port_forwards="21433:1433",
    labels=["infra"],
    resource_deps=["namespace"])

k8s_resource("opensearch",
    labels=["infra"],
    resource_deps=["namespace"],
    links=[link("http://opensearch.localhost:9080", "OpenSearch")])

k8s_resource("wiremock",
    port_forwards="8080:8080",
    labels=["infra"],
    resource_deps=["wiremock-mappings-configmap", "wiremock-files-configmap"],
    links=[link("http://mock.localhost:9080", "WireMock")],
    pod_readiness="ignore")

k8s_resource("seq",
    labels=["infra"],
    resource_deps=["namespace"],
    links=[link("http://seq.localhost:9080", "Seq")])

k8s_resource("otel-collector",
    labels=["infra"],
    resource_deps=["seq"],
    pod_readiness="wait")

k8s_resource("keycloak",
    labels=["infra"],
    resource_deps=["keycloak-realm-configmap"],
    links=[link("http://keycloak.localhost:9080", "Keycloak")])

k8s_resource(
    objects=[
        "wegwijs-demo:ingressroute",
        "wegwijs-demo-tls:ingressroute",
        "https-to-http:middleware",
    ],
    new_name="ingress-routes",
    resource_deps=["wait-traefik-crds"],
    labels=["infra"],
    pod_readiness="ignore"
)

# =============================================================================
# Image builds
# =============================================================================

docker_build(
    "wegwijs-api:local",
    ".",
    dockerfile="api/Dockerfile",
    only=[
        "api/Dockerfile",
        ".config/dotnet-tools.json",
        "SolutionInfo.cs",
        "organisationregistry-api.pfx",
        "Directory.Build.props",
        "Directory.Packages.props",
        "global.json",
        "OrganisationRegistry.sln",
        "src/OrganisationRegistry",
        "src/OrganisationRegistry.Api",
        "src/OrganisationRegistry.Configuration.Database",
        "src/OrganisationRegistry.ElasticSearch",
        "src/OrganisationRegistry.Infrastructure",
        "src/OrganisationRegistry.Magda",
        "src/OrganisationRegistry.OpenTelemetry",
        "src/OrganisationRegistry.SqlServer",
        "src/Osc",
        "src/OpenSearch.Net",
    ],
    ignore=["**/bin", "**/obj"],
)

custom_build(
  image_ref('ui'),
    'docker build -f src/OrganisationRegistry.UI/Dockerfile.optimized -t $EXPECTED_REF .',
    # image_ref("ui"),
    # 'DOCKER_BUILDKIT=1 docker build -f src/OrganisationRegistry.UI/Dockerfile.optimized -t $EXPECTED_REF . && docker tag $EXPECTED_REF localhost:5051/wegwijs-ui:$EXPECTED_TAG && docker push localhost:5051/wegwijs-ui:$EXPECTED_TAG',
    deps=[
        "src/OrganisationRegistry.UI/Dockerfile.optimized",
        "src/OrganisationRegistry.UI/app",
        "src/OrganisationRegistry.UI/assets",
        "src/OrganisationRegistry.UI/Infrastructure",
        "src/OrganisationRegistry.UI/default.conf",
        "src/OrganisationRegistry.UI/init.sh",
        "src/OrganisationRegistry.UI/config.js",
        "src/OrganisationRegistry.UI/index.html",
        "src/OrganisationRegistry.UI/main.aot.ts",
        "src/OrganisationRegistry.UI/main.browser.ts",
        "src/OrganisationRegistry.UI/polyfills.browser.ts",
        "src/OrganisationRegistry.UI/vendor.browser.ts",
        "src/OrganisationRegistry.UI/custom-typings.d.ts",
        "package.json",
        "package-lock.json",
        "tsconfig.aot.json",
        "config/",
        "scripts/",
        "organisationregistry-ui.pfx",
    ],
    ignore=["**/node_modules", "**/bin", "**/obj", "**/*.map", "src/OrganisationRegistry.UI/wwwroot", "src/OrganisationRegistry.UI/dist"],
    disable_push=False,
)

docker_build(
    image_ref("piavo-import"),
    ".",
    dockerfile="test/OrganisationRegistry.Import.Piavo/Dockerfile",
    only=[
        "test/OrganisationRegistry.Import.Piavo/Dockerfile",
        ".config/dotnet-tools.json",
        "organisationregistry-api.pfx",
        "organisationregistry-ui.pfx",
        "SolutionInfo.cs",
        "Directory.Build.props",
        "Directory.Packages.props",
        "global.json",
        "OrganisationRegistry.sln",
        "src/OrganisationRegistry",
        "src/OrganisationRegistry.Api",
        "src/OrganisationRegistry.Configuration.Database",
        "src/OrganisationRegistry.ElasticSearch",
        "src/OrganisationRegistry.Infrastructure",
        "src/OrganisationRegistry.Magda",
        "src/OrganisationRegistry.OpenTelemetry",
        "src/OrganisationRegistry.SqlServer",
        "src/Osc",
        "src/OpenSearch.Net",
        "test/OrganisationRegistry.Import.Piavo/",
    ],
    ignore=["**/bin", "**/obj"],
)

custom_build(image_ref("m2m"), "docker build -t $EXPECTED_REF demo/m2m && docker push $EXPECTED_REF", deps=["demo/m2m/"])
custom_build(image_ref("nuxt-bff"), "docker build -t $EXPECTED_REF demo/nuxt-bff && docker push $EXPECTED_REF", deps=["demo/nuxt-bff/"])
custom_build(image_ref("seed"), "docker build -t $EXPECTED_REF demos/seed && docker push $EXPECTED_REF", deps=["demos/seed/"])

# =============================================================================
# Apps
# =============================================================================

k8s_resource("api",
    labels=["apps"],
    resource_deps=["clear-database", "mssql", "opensearch", "keycloak", "wiremock", "otel-collector"],
    links=[link("http://api.localhost:9080/v1", "API")],
    auto_init=True,
    trigger_mode=TRIGGER_MODE_MANUAL)

k8s_resource("ui",
    labels=["apps"],
    links=[link("http://ui.localhost:9080", "UI")])

k8s_resource("m2m-demo",
    labels=["apps"],
    links=[link("http://m2m.localhost:9080", "M2M Demo")],
    auto_init=True)

k8s_resource("nuxt-bff",
    labels=["apps"],
    links=[link("http://app.localhost:9080", "Nuxt BFF")],
    auto_init=True)

cmd_button("api:connect", argv=["./scripts/tunnel.sh", "--resource", "api", "--state", "on"], resource="api", icon_name="verified_user", text="Tunnel connect")
cmd_button("api:disconnect", argv=["./scripts/tunnel.sh", "--resource", "api", "--state", "off"], resource="api", icon_name="close", text="Tunnel disconnect")

# =============================================================================
# Bootstrap
# =============================================================================

k8s_resource("piavo-import",
    labels=["setup"],
    resource_deps=["api-configuration", "seed"],
    auto_init=True,
    trigger_mode=TRIGGER_MODE_MANUAL)

k8s_resource("seed",
    labels=["setup"],
    resource_deps=["api-configuration", "keycloak"])

print("")
print("╔═══════════════════════════════════════════════════════════════╗")
print("║  Wegwijs / Organisation Registry - Development Environment    ║")
print("╠═══════════════════════════════════════════════════════════════╣")
print("║  keycloak.localhost:9080  → Keycloak (admin/admin)            ║")
print("║  seq.localhost:9080       → Seq (structured logs / OTLP)      ║")
print("║  opensearch.localhost:9080 → OpenSearch                       ║")
print("║  mock.localhost:9080      → WireMock (MAGDA mock)             ║")
print("║  api.localhost:9080       → Organisation Registry API         ║")
print("║  ui.localhost:9080        → Angular UI (backoffice)           ║")
print("║  m2m.localhost:9080       → M2M demo (client credentials)     ║")
print("║  app.localhost:9080       → Nuxt BFF (Keycloak demo)          ║")
print("╠═══════════════════════════════════════════════════════════════╣")
print("║  Demo users: dev / vlimpers / algemeenbeheerder (pw = user)   ║")
print("╚═══════════════════════════════════════════════════════════════╝")
print("")
