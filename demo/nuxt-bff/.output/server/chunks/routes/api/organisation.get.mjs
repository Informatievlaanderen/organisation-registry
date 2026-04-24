import { d as defineEventHandler, g as getQuery, c as createError, u as useRuntimeConfig } from '../../nitro/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

const organisation_get = defineEventHandler(async (event) => {
  var _a, _b;
  const config = useRuntimeConfig();
  const query = getQuery(event);
  const ovo = query.ovo;
  if (!ovo) {
    throw createError({ statusCode: 400, statusMessage: "ovo parameter is verplicht" });
  }
  const url = `${config.apiBaseUrl}/organisations/${ovo}`;
  try {
    return await $fetch(url, { headers: { Accept: "application/json" } });
  } catch (err) {
    throw createError({
      statusCode: (_b = (_a = err == null ? void 0 : err.response) == null ? void 0 : _a.status) != null ? _b : 502,
      statusMessage: `Organisatie niet gevonden: ${ovo}`
    });
  }
});

export { organisation_get as default };
//# sourceMappingURL=organisation.get.mjs.map
