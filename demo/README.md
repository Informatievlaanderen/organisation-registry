# Demo environment

De demo draait lokaal via `k3d` + `Tilt`.

In deze setup:
- Keycloak vervangt ACM/IDM als lokale identity provider
- Traefik routeert alle demo-apps via `*.localhost:9080`
- de workloads draaien als Kubernetes resources uit `demo/k8s/`

## Vereisten

- docker
- k3d
- kubectl
- helm
- tilt

## Opstarten

Snelste pad:

```bash
bash scripts/dev-setup.sh
```

Manueel:

```bash
k3d cluster create --config k3d.config.yaml
helm upgrade --install traefik traefik/traefik \
  -f demo/helm/traefik-values.yaml \
  -n traefik \
  --create-namespace
tilt up
```

## Beschikbare URLs

- `http://keycloak.localhost:9080`
- `http://seq.localhost:9080`
- `http://opensearch.localhost:9080`
- `http://mock.localhost:9080`
- `http://api.localhost:9080/v1`
- `http://ui.localhost:9080`
- `http://m2m.localhost:9080`
- `http://app.localhost:9080`

## Relevante bestanden

- [`Tiltfile`](../Tiltfile)
- [`k3d.config.yaml`](../k3d.config.yaml)
- [`demo/k8s`](k8s/config.yaml)
- [`scripts/dev-setup.sh`](../scripts/dev-setup.sh)
