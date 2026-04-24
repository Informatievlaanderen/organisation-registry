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

const me_get = defineEventHandler(async (event) => {
  var _a;
  const config = useRuntimeConfig();
  const session = getSession(event, config.sessionSecret);
  if (!session.accessToken) {
    return { loggedIn: false };
  }
  try {
    const parts = session.accessToken.split(".");
    const payload = JSON.parse(Buffer.from(parts[1], "base64").toString("utf8"));
    return {
      loggedIn: true,
      sub: payload.sub,
      clientId: payload.azp || payload.client_id,
      scope: payload.scope,
      exp: payload.exp,
      // Token exchange status
      tokenExchange: session.exchangedToken ? { success: true } : { success: false, error: (_a = session.exchangeError) != null ? _a : "onbekende fout" }
    };
  } catch {
    return { loggedIn: false };
  }
});

export { me_get as default };
//# sourceMappingURL=me.get.mjs.map
