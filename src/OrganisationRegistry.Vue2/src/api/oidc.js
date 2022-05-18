import Oidc from "oidc-client";
import { removeItem } from "@/api/localStorage";

class OidcClient {
  static createSettings(data) {
    return {
      authority: data.authority, // 'https://authenticatie-ti.vlaanderen.be/op',
      metadata: {
        issuer: data.issuer, // 'https://authenticatie-ti.vlaanderen.be/op',
        authorization_endpoint: data.authorizationEndpoint, // 'https://authenticatie-ti.vlaanderen.be/op/v1/auth',
        userinfo_endpoint: data.userInfoEndPoint, // 'https://authenticatie-ti.vlaanderen.be/op/v1/userinfo',
        end_session_endpoint: data.endSessionEndPoint, // 'https://authenticatie-ti.vlaanderen.be/op/v1/logout',
        jwks_uri: data.jwks_uri, // 'https://authenticatie-ti.vlaanderen.be/op/v1/keys',
      },
      signing_keys: ["RS256"],

      client_id: data.clientId, // 'dcdf573f-b430-4d34-b2f1-218af8506ba4',
      redirect_uri: data.redirectUri.replace("/#/", "/"), // 'https://dienstverlening.staging-basisregisters.vlaanderen/oic',
      post_logout_redirect_uri: data.postLogoutRedirectUri, // 'https://dienstverlening.staging-basisregisters.vlaanderen',
      response_type: "code",
      scope: "openid profile vo iv_wegwijs",
      filterProtocolClaims: true,
      loadUserInfo: true,
      query_status_response_type: "code",
    };
  }

  constructor(data) {
    const settings = OidcClient.createSettings(data);
    this.client = new Oidc.OidcClient(settings);
  }

  signIn() {
    this.client
      .createSigninRequest({
        state: {
          bar: 15,
        },
      })
      .then((req) => {
        localStorage.setItem("verifier", req.state.code_verifier);
        window.location = req.url;
      })
      .catch((err) => {
        console.log("Shriek!", err);
        console.log("Shriek!", err.request);
      });
  }

  signOut() {
    removeItem("token");
    this.client
      .createSignoutRequest({
        state: {
          bar: 15,
        },
      })
      .then((req) => {
        window.location = req.url;
      })
      .catch((err) => {
        console.log("Shriek!", err);
        console.log("Shriek!", err.request);
      });
  }
}

export default OidcClient;
