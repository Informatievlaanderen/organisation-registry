FROM node:20-alpine AS builder

WORKDIR /app

COPY package.json ./
COPY package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# ---- runtime ----
FROM node:20-alpine

WORKDIR /app
COPY --from=builder /app/.output ./.output

ENV NODE_ENV=production
ENV HOST=0.0.0.0
ENV PORT=3000

EXPOSE 3000

CMD ["node", ".output/server/index.mjs"]
