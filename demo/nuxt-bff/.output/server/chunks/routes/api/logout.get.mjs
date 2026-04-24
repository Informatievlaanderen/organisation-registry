import { d as defineEventHandler, s as sendRedirect, u as useRuntimeConfig } from '../../nitro/nitro.mjs';
import { c as clearSession } from '../../_/session.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';
import 'crypto';

const logout_get = defineEventHandler((event) => {
  const config = useRuntimeConfig();
  clearSession(event);
  const logoutUrl = `${config.public.keycloakUrl}/realms/${config.public.keycloakRealm}/protocol/openid-connect/logout?client_id=${config.public.keycloakClientId}&post_logout_redirect_uri=${encodeURIComponent(config.public.appBaseUrl)}`;
  return sendRedirect(event, logoutUrl);
});

export { logout_get as default };
//# sourceMappingURL=logout.get.mjs.map
