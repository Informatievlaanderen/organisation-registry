export function getToken() {
  return window.localStorage.token;
}

export function setToken(value) {
  window.localStorage.token = value;
}

export function removeItem(item) {
  window.localStorage.removeItem(item);
}
