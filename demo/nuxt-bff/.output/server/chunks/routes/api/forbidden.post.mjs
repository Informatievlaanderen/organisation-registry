import { d as defineEventHandler, u as useRuntimeConfig } from '../../nitro/nitro.mjs';
import { g as getSession } from '../../_/session.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';
import 'crypto';

const forbidden_post = defineEventHandler(async (event) => {
  var _a, _b, _c, _d, _e, _f;
  const config = useRuntimeConfig();
  const session = getSession(event, config.sessionSecret);
  if (!session.accessToken) {
    return { error: "Niet ingelogd", status: 401 };
  }
  const forbiddenUrl = `${config.apiBaseUrl}/events`;
  let status = 0;
  let statusText = "";
  let responseBody = null;
  try {
    await $fetch(forbiddenUrl, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${session.accessToken}`,
        Accept: "application/json"
      }
    });
    status = 200;
    statusText = "OK";
    responseBody = { success: true };
  } catch (err) {
    status = (_b = (_a = err == null ? void 0 : err.response) == null ? void 0 : _a.status) != null ? _b : 502;
    statusText = (_e = (_d = (_c = err == null ? void 0 : err.response) == null ? void 0 : _c.statusText) != null ? _d : err == null ? void 0 : err.message) != null ? _e : "Fout";
    try {
      responseBody = await ((_f = err == null ? void 0 : err.response) == null ? void 0 : _f.json());
    } catch {
      responseBody = err == null ? void 0 : err.message;
    }
  }
  return {
    status,
    statusText,
    body: responseBody,
    request: {
      method: "GET",
      url: forbiddenUrl,
      authScheme: "Bearer (Keycloak access token, server-side)"
    }
  };
});

export { forbidden_post as default };
//# sourceMappingURL=forbidden.post.mjs.map
