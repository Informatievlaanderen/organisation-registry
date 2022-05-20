export function getToken() {
  return window.localStorage.token;
}

export function setToken(value) {
  window.localStorage.token = value;
}

export function removeToken() {
  return removeItem("token");
}

export function getVerifier() {
  return window.localStorage.verifier;
}

export function removeItem(item) {
  window.localStorage.removeItem(item);
}
