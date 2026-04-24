import { a as getCookie, b as setCookie } from '../nitro/nitro.mjs';
import { createDecipheriv, randomBytes, createCipheriv } from 'crypto';

const COOKIE_NAME = "bff_session";
const COOKIE_NAME_EX = "bff_session_ex";
const COOKIE_MAX_AGE = 60 * 60;
function getKey(secret) {
  const buf = Buffer.alloc(32, 0);
  Buffer.from(secret).copy(buf, 0, 0, 32);
  return buf;
}
function encrypt(data, secret) {
  const iv = randomBytes(16);
  const key = getKey(secret);
  const cipher = createCipheriv("aes-256-cbc", key, iv);
  const json = JSON.stringify(data);
  const encrypted = Buffer.concat([cipher.update(json, "utf8"), cipher.final()]);
  return iv.toString("base64") + "." + encrypted.toString("base64");
}
function decrypt(token, secret) {
  try {
    const isNewFormat = token.includes(".");
    const [ivPart, encPart] = token.split(isNewFormat ? "." : ":");
    const iv = Buffer.from(ivPart, isNewFormat ? "base64" : "hex");
    const encrypted = Buffer.from(encPart, isNewFormat ? "base64" : "hex");
    const key = getKey(secret);
    const decipher = createDecipheriv("aes-256-cbc", key, iv);
    const decrypted = Buffer.concat([decipher.update(encrypted), decipher.final()]);
    return JSON.parse(decrypted.toString("utf8"));
  } catch {
    return null;
  }
}
function getSession(event, secret) {
  const cookie1 = getCookie(event, COOKIE_NAME);
  const cookie2 = getCookie(event, COOKIE_NAME_EX);
  const part1 = cookie1 ? decrypt(cookie1, secret) : null;
  const part2 = cookie2 ? decrypt(cookie2, secret) : null;
  return {
    accessToken: part1 == null ? void 0 : part1.accessToken,
    exchangeError: part1 == null ? void 0 : part1.exchangeError,
    exchangedToken: part2 == null ? void 0 : part2.exchangedToken
  };
}
function saveSession(event, session, secret) {
  const cookieOptions = {
    httpOnly: true,
    secure: false,
    // HTTP for local development
    sameSite: "lax",
    maxAge: COOKIE_MAX_AGE,
    path: "/"
  };
  const part1 = {
    accessToken: session.accessToken,
    exchangeError: session.exchangeError
  };
  const encrypted1 = encrypt(part1, secret);
  console.log("[session] Part 1 (accessToken) length:", encrypted1.length);
  setCookie(event, COOKIE_NAME, encrypted1, cookieOptions);
  if (session.exchangedToken) {
    const part2 = {
      exchangedToken: session.exchangedToken
    };
    const encrypted2 = encrypt(part2, secret);
    console.log("[session] Part 2 (exchangedToken) length:", encrypted2.length);
    setCookie(event, COOKIE_NAME_EX, encrypted2, cookieOptions);
  }
}
function clearSession(event) {
  const cookieOptions = {
    httpOnly: true,
    secure: false,
    sameSite: "lax",
    maxAge: 0,
    path: "/"
  };
  setCookie(event, COOKIE_NAME, "", cookieOptions);
  setCookie(event, COOKIE_NAME_EX, "", cookieOptions);
}

export { clearSession as c, getSession as g, saveSession as s };
//# sourceMappingURL=session.mjs.map
