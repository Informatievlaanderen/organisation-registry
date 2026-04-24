import { d as defineEventHandler, s as sendRedirect, u as useRuntimeConfig } from '../../nitro/nitro.mjs';
import { randomBytes, createHash, createCipheriv } from 'crypto';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

function base64url(buf) {
  return buf.toString("base64").replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}
function getKey(secret) {
  const buf = Buffer.alloc(32, 0);
  Buffer.from(secret).copy(buf, 0, 0, 32);
  return buf;
}
function encryptState(data, secret) {
  const iv = randomBytes(16);
  const key = getKey(secret);
  const cipher = createCipheriv("aes-256-cbc", key, iv);
  const json = JSON.stringify(data);
  const encrypted = Buffer.concat([cipher.update(json, "utf8"), cipher.final()]);
  const combined = Buffer.concat([iv, encrypted]);
  return base64url(combined);
}
const login_get = defineEventHandler(async (event) => {
  const config = useRuntimeConfig();
  const nonce = base64url(randomBytes(16));
  const codeVerifier = base64url(randomBytes(32));
  const codeChallenge = base64url(
    createHash("sha256").update(codeVerifier).digest()
  );
  const state = encryptState({ nonce, codeVerifier }, config.sessionSecret);
  const params = new URLSearchParams({
    response_type: "code",
    client_id: config.public.keycloakClientId,
    redirect_uri: `${config.public.appBaseUrl}/callback`,
    scope: "openid profile dv_organisatieregister_cjmbeheerder",
    state,
    code_challenge: codeChallenge,
    code_challenge_method: "S256"
  });
  const authUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/auth?${params}`;
  return sendRedirect(event, authUrl);
});

export { login_get as default };
//# sourceMappingURL=login.get.mjs.map
