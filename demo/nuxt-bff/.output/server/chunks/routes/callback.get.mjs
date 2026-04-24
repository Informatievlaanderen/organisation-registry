import { d as defineEventHandler, g as getQuery, s as sendRedirect, c as createError, u as useRuntimeConfig } from '../nitro/nitro.mjs';
import { c as clearSession, s as saveSession } from '../_/session.mjs';
import { createDecipheriv } from 'crypto';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

function base64urlToBuffer(str) {
  const pad = (4 - str.length % 4) % 4;
  const b64 = str.replace(/-/g, "+").replace(/_/g, "/") + "=".repeat(pad);
  return Buffer.from(b64, "base64");
}
function getKey(secret) {
  const buf = Buffer.alloc(32, 0);
  Buffer.from(secret).copy(buf, 0, 0, 32);
  return buf;
}
function decryptState(state, secret) {
  try {
    const combined = base64urlToBuffer(state);
    const iv = combined.subarray(0, 16);
    const encrypted = combined.subarray(16);
    const key = getKey(secret);
    const decipher = createDecipheriv("aes-256-cbc", key, iv);
    const decrypted = Buffer.concat([decipher.update(encrypted), decipher.final()]);
    return JSON.parse(decrypted.toString("utf8"));
  } catch {
    return null;
  }
}
const callback_get = defineEventHandler(async (event) => {
  var _a, _b, _c, _d, _e;
  console.log("[callback] Handler started");
  const config = useRuntimeConfig();
  const query = getQuery(event);
  console.log("[callback] Query params:", { code: query.code ? "present" : "missing", state: query.state ? "present" : "missing", error: query.error });
  const code = query.code;
  const state = query.state;
  const error = query.error;
  if (error) {
    clearSession(event);
    return sendRedirect(event, `/?error=${encodeURIComponent(String(query.error_description || error))}`);
  }
  if (!code || !state) {
    clearSession(event);
    return sendRedirect(event, `/?error=missing_code_or_state`);
  }
  const stateData = decryptState(state, config.sessionSecret);
  if (!stateData || !stateData.codeVerifier) {
    clearSession(event);
    return sendRedirect(event, `/?error=invalid_state`);
  }
  const { codeVerifier } = stateData;
  const keycloakTokenUrl = `${config.keycloakInternalUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/token`;
  const tokenRes = await $fetch(keycloakTokenUrl, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: new URLSearchParams({
      grant_type: "authorization_code",
      client_id: config.public.keycloakClientId,
      client_secret: config.keycloakClientSecret,
      redirect_uri: `${config.public.appBaseUrl}/callback`,
      code,
      code_verifier: codeVerifier
    }).toString()
  }).catch((err) => {
    throw createError({ statusCode: 502, statusMessage: `Keycloak code exchange failed: ${err.message}` });
  });
  const accessToken = tokenRes.access_token;
  let exchangedToken;
  let exchangeError;
  try {
    const exchangeRes = await $fetch(keycloakTokenUrl, {
      method: "POST",
      headers: { "Content-Type": "application/x-www-form-urlencoded" },
      body: new URLSearchParams({
        grant_type: "urn:ietf:params:oauth:grant-type:token-exchange",
        client_id: config.public.keycloakClientId,
        client_secret: config.keycloakClientSecret,
        subject_token: accessToken,
        subject_token_type: "urn:ietf:params:oauth:token-type:access_token",
        requested_token_type: "urn:ietf:params:oauth:token-type:access_token",
        audience: "organisation-registry-api"
      }).toString()
    });
    exchangedToken = exchangeRes.access_token;
  } catch (err) {
    const errData = (_b = err == null ? void 0 : err.data) != null ? _b : (_a = err == null ? void 0 : err.response) == null ? void 0 : _a._data;
    exchangeError = (_e = (_d = (_c = errData == null ? void 0 : errData.error_description) != null ? _c : errData == null ? void 0 : errData.error) != null ? _d : err == null ? void 0 : err.message) != null ? _e : "token exchange failed";
  }
  console.log("[callback] Saving session with tokens:", {
    hasAccessToken: !!accessToken,
    hasExchangedToken: !!exchangedToken,
    exchangeError
  });
  saveSession(event, {
    accessToken,
    exchangedToken,
    exchangeError}, config.sessionSecret);
  console.log("[callback] Session saved, redirecting to /");
  return sendRedirect(event, "/");
});

export { callback_get as default };
//# sourceMappingURL=callback.get.mjs.map
