import jwtDecode from "jwt-decode";

export class UserTokenResult {
  constructor(decodedJwtToken, succeeded) {
    this.succeeded = succeeded;
    if (succeeded) {
      this.token = decodedJwtToken;
      this.user = {
        name: decodedJwtToken.family_name,
        firstName: decodedJwtToken.given_name,
        roles: decodedJwtToken.role,
        translatedRoles: []
          .concat(decodedJwtToken.role)
          .map((x) => UserTokenResult.#translateRole(x)),
      };
    }
  }

  get expiration() {
    return this.token.exp;
  }

  get isExpired() {
    return Date.now() >= this.expiration * 1000;
  }

  static #translateRole(role) {
    switch (role) {
      case "algemeenBeheerder":
        return "Algemeen beheerder";
      case "vlimpersBeheerder":
        return "Vlimpers beheerder";
      case "decentraalBeheerder":
        return "Decentraal Beheerder";
      case "orgaanBeheerder":
        return "Orgaan Beheerder";
      case "developer":
        return "Ontwikkelaar";
      case "regelgevingBeheerder": {
        return "Regelgeving en deugdelijk bestuur beheerder";
      }
    }
    return "";
  }

  static fromJwt(jwt) {
    if (!jwt) return new UserTokenResult(null, false);

    try {
      return new UserTokenResult(jwtDecode(jwt), true);
    } catch (e) {
      console.error(e);
      return new UserTokenResult(null, false);
    }
  }
}
