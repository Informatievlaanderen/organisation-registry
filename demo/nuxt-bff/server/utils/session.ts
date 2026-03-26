/**
 * Sessie-utilities — sla tokens op in een encrypted cookie.
 * Voor de demo gebruiken we een simpele AES-256-CBC encryptie
 * (tokens komen nooit in de browser response — BFF-patroon).
 *
 * Velden in de sessie:
 *   - accessToken:    het Keycloak access token (voor de gebruiker)
 *   - exchangedToken: het via RFC 8693 token exchange verkregen token
 *                     gericht op organisation-registry-api audience
 *   - exchangeError:  foutmelding als de token exchange mislukt
 *   - codeVerifier:   PKCE verifier (tijdelijk, enkel tijdens login flow)
 *   - state:          CSRF state parameter
 */

import { H3Event, setCookie, getCookie } from 'h3'
import { createCipheriv, createDecipheriv, randomBytes } from 'crypto'

const COOKIE_NAME = 'bff_session'
const COOKIE_MAX_AGE = 60 * 60 // 1 uur

export interface Session {
  accessToken?: string
  exchangedToken?: string
  exchangeError?: string
  codeVerifier?: string
  state?: string
}

function getKey(secret: string): Buffer {
  // Pad/truncate secret to 32 bytes for AES-256
  const buf = Buffer.alloc(32, 0)
  Buffer.from(secret).copy(buf, 0, 0, 32)
  return buf
}

export function encryptSession(data: Session, secret: string): string {
  const iv = randomBytes(16)
  const key = getKey(secret)
  const cipher = createCipheriv('aes-256-cbc', key, iv)
  const json = JSON.stringify(data)
  const encrypted = Buffer.concat([cipher.update(json, 'utf8'), cipher.final()])
  return iv.toString('hex') + ':' + encrypted.toString('hex')
}

export function decryptSession(token: string, secret: string): Session {
  try {
    const [ivHex, encHex] = token.split(':')
    const iv = Buffer.from(ivHex, 'hex')
    const key = getKey(secret)
    const decipher = createDecipheriv('aes-256-cbc', key, iv)
    const decrypted = Buffer.concat([decipher.update(Buffer.from(encHex, 'hex')), decipher.final()])
    return JSON.parse(decrypted.toString('utf8'))
  } catch {
    return {}
  }
}

export function getSession(event: H3Event, secret: string): Session {
  const cookie = getCookie(event, COOKIE_NAME)
  if (!cookie) return {}
  return decryptSession(cookie, secret)
}

export function saveSession(event: H3Event, session: Session, secret: string): void {
  const encrypted = encryptSession(session, secret)
  setCookie(event, COOKIE_NAME, encrypted, {
    httpOnly: true,
    sameSite: 'lax',
    maxAge: COOKIE_MAX_AGE,
    path: '/',
  })
}

export function clearSession(event: H3Event): void {
  setCookie(event, COOKIE_NAME, '', {
    httpOnly: true,
    sameSite: 'lax',
    maxAge: 0,
    path: '/',
  })
}
