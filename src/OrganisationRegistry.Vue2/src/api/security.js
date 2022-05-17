import jwtDecode from "jwt-decode";

export function getToken() {
  return window.localStorage.token;
}

export function setToken(value) {
  window.localStorage.token = value;
}

export function removeItem(item) {
  window.localStorage.removeItem(item);
}

export function loadUser(store) {
  const token = getToken();
  if (token) {
    const decoded = jwtDecode(token);
    store.setUser(decoded);
  } else {
    store.clearUser();
  }
}
