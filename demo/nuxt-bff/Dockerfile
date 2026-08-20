FROM node:22.23-alpine3.23 AS deps
WORKDIR /workspace

RUN --mount=type=bind,source=package.json,target=package.json \
    --mount=type=bind,source=package-lock.json,target=package-lock.json \
    --mount=type=cache,target=/root/.npm,sharing=locked \
    npm ci --no-audit --no-fund

FROM deps AS builder
WORKDIR /workspace
COPY . .
RUN --mount=type=cache,target=/workspace/node_modules/.cache \
    npm run build

# ---- dev (Tilt live_update target, not for deploy) ----
FROM deps AS dev
WORKDIR /workspace
COPY . .
ENV NODE_ENV=development \
    HOST=0.0.0.0 \
    PORT=3000
EXPOSE 3000
CMD ["npm", "run", "dev"]

FROM node:22.23-alpine3.23 AS runtime
WORKDIR /app

ENV NODE_ENV=production \
    HOST=0.0.0.0 \
    PORT=3000

COPY --link --from=builder /workspace/.output ./.output

USER node
EXPOSE 3000
CMD ["node", ".output/server/index.mjs"]
