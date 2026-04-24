import { d as defineEventHandler, r as readBody, u as useRuntimeConfig } from '../../nitro/nitro.mjs';
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

const allowed_post = defineEventHandler(async (event) => {
  var _a, _b, _c, _d, _e, _f, _g, _h, _i, _j, _k, _l, _m, _n, _o, _p, _q, _r;
  const config = useRuntimeConfig();
  const session = getSession(event, config.sessionSecret);
  if (!session.accessToken) {
    return { error: "Niet ingelogd", status: 401 };
  }
  const body = await readBody(event);
  const organisationId = body == null ? void 0 : body.organisationId;
  const newName = body == null ? void 0 : body.name;
  if (!organisationId || !newName) {
    return { error: "organisationId en name zijn verplicht", status: 400 };
  }
  const bearerToken = (_a = session.exchangedToken) != null ? _a : session.accessToken;
  const tokenSource = session.exchangedToken ? "exchanged (RFC 8693)" : "direct Keycloak access token (exchange mislukt)";
  let currentOrg;
  try {
    currentOrg = await $fetch(`${config.apiBaseUrl}/organisations/${organisationId}`, {
      headers: { Accept: "application/json" }
    });
  } catch (err) {
    return {
      status: (_c = (_b = err == null ? void 0 : err.response) == null ? void 0 : _b.status) != null ? _c : 502,
      statusText: "Ophalen organisatie mislukt",
      error: err == null ? void 0 : err.message
    };
  }
  const putUrl = `${config.apiBaseUrl}/organisations/${organisationId}`;
  let status = 0;
  let statusText = "";
  let responseBody = null;
  try {
    await $fetch(putUrl, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${bearerToken}`,
        "Content-Type": "application/json",
        Accept: "application/json"
      },
      body: JSON.stringify({
        name: newName,
        description: (_d = currentOrg.description) != null ? _d : "",
        shortName: (_e = currentOrg.shortName) != null ? _e : "",
        purposeIds: (_f = currentOrg.purposeIds) != null ? _f : [],
        showOnVlaamseOverheidSites: (_g = currentOrg.showOnVlaamseOverheidSites) != null ? _g : false,
        validFrom: (_h = currentOrg.validFrom) != null ? _h : null,
        validTo: (_i = currentOrg.validTo) != null ? _i : null,
        article: (_j = currentOrg.article) != null ? _j : null,
        operationalValidFrom: (_k = currentOrg.operationalValidFrom) != null ? _k : null,
        operationalValidTo: (_l = currentOrg.operationalValidTo) != null ? _l : null
      })
    });
    status = 200;
    statusText = "OK";
    responseBody = { success: true };
  } catch (err) {
    status = (_n = (_m = err == null ? void 0 : err.response) == null ? void 0 : _m.status) != null ? _n : 502;
    statusText = (_q = (_p = (_o = err == null ? void 0 : err.response) == null ? void 0 : _o.statusText) != null ? _p : err == null ? void 0 : err.message) != null ? _q : "Fout";
    try {
      responseBody = await ((_r = err == null ? void 0 : err.response) == null ? void 0 : _r.json());
    } catch {
      responseBody = err == null ? void 0 : err.message;
    }
  }
  return {
    status,
    statusText,
    body: responseBody,
    tokenExchangeUsed: !!session.exchangedToken,
    tokenSource,
    note: status === 401 ? "De API weigert het token \u2014 [OrganisationRegistryAuthorize] is gebonden aan het custom JwtBearer scheme, niet aan EditApi (OAuth2 introspection). Dit is het verwachte gedrag dat de demo aantoont." : null,
    request: {
      method: "PUT",
      url: putUrl,
      authScheme: `Bearer (${tokenSource}, server-side)`
    }
  };
});

export { allowed_post as default };
//# sourceMappingURL=allowed.post.mjs.map
