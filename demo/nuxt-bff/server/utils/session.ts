/**
 * Sessie-utilities — sla tokens op in encrypted cookies.
 * 
 * Omdat JWT tokens groot zijn (vaak >2KB elk), splitsen we de sessie
 * over twee cookies om binnen browser limieten te blijven:
 *   - bff_session: bevat accessToken + metadata
 *   - bff_session_ex: bevat exchangedToken (van token exchange)
 * 
 * Beide cookies zijn HttpOnly en encrypted met AES-256-CBC.
 */

import { H3Event, setCookie, getCookie } from 'h3'
import { createCipheriv, createDecipheriv, randomBytes } from 'crypto'

const COOKIE_NAME = 'bff_session'
const COOKIE_NAME_EX = 'bff_session_ex'  // For exchanged token
const COOKIE_MAX_AGE = 60 * 60 // 1 uur

export interface Session {
  accessToken?: string
  exchangedToken?: string
  exchangeError?: string
  codeVerifier?: string
  state?: string
}

interface SessionPart1 {
  accessToken?: string
  exchangeError?: string
}

interface SessionPart2 {
  exchangedToken?: string
}

function getKey(secret: string): Buffer {
  // Pad/truncate secret to 32 bytes for AES-256
  const buf = Buffer.alloc(32, 0)
  Buffer.from(secret).copy(buf, 0, 0, 32)
  return buf
}

function encrypt(data: object, secret: string): string {
  const iv = randomBytes(16)
  const key = getKey(secret)
  const cipher = createCipheriv('aes-256-cbc', key, iv)
  const json = JSON.stringify(data)
  const encrypted = Buffer.concat([cipher.update(json, 'utf8'), cipher.final()])
  // Use base64 encoding to save space
  return iv.toString('base64') + '.' + encrypted.toString('base64')
}

function decrypt<T>(token: string, secret: string): T | null {
  try {
    // Support both old hex format and new base64 format
    const isNewFormat = token.includes('.')
    const [ivPart, encPart] = token.split(isNewFormat ? '.' : ':')
    const iv = Buffer.from(ivPart, isNewFormat ? 'base64' : 'hex')
    const encrypted = Buffer.from(encPart, isNewFormat ? 'base64' : 'hex')
    const key = getKey(secret)
    const decipher = createDecipheriv('aes-256-cbc', key, iv)
    const decrypted = Buffer.concat([decipher.update(encrypted), decipher.final()])
    return JSON.parse(decrypted.toString('utf8'))
  } catch {
    return null
  }
}

export function getSession(event: H3Event, secret: string): Session {
  const cookie1 = getCookie(event, COOKIE_NAME)
  const cookie2 = getCookie(event, COOKIE_NAME_EX)
  
  const part1 = cookie1 ? decrypt<SessionPart1>(cookie1, secret) : null
  const part2 = cookie2 ? decrypt<SessionPart2>(cookie2, secret) : null
  
  return {
    accessToken: part1?.accessToken,
    exchangeError: part1?.exchangeError,
    exchangedToken: part2?.exchangedToken,
  }
}

export function saveSession(event: H3Event, session: Session, secret: string): void {
  const cookieOptions = {
    httpOnly: true,
    secure: false,  // HTTP for local development
    sameSite: 'lax' as const,
    maxAge: COOKIE_MAX_AGE,
    path: '/',
  }
  
  // Part 1: accessToken + metadata (in main cookie)
  const part1: SessionPart1 = {
    accessToken: session.accessToken,
    exchangeError: session.exchangeError,
  }
  const encrypted1 = encrypt(part1, secret)
  console.log('[session] Part 1 (accessToken) length:', encrypted1.length)
  setCookie(event, COOKIE_NAME, encrypted1, cookieOptions)
  
  // Part 2: exchangedToken (in separate cookie, may be empty)
  if (session.exchangedToken) {
    const part2: SessionPart2 = {
      exchangedToken: session.exchangedToken,
    }
    const encrypted2 = encrypt(part2, secret)
    console.log('[session] Part 2 (exchangedToken) length:', encrypted2.length)
    setCookie(event, COOKIE_NAME_EX, encrypted2, cookieOptions)
  }
}

export function clearSession(event: H3Event): void {
  const cookieOptions = {
    httpOnly: true,
    secure: false,
    sameSite: 'lax' as const,
    maxAge: 0,
    path: '/',
  }
  setCookie(event, COOKIE_NAME, '', cookieOptions)
  setCookie(event, COOKIE_NAME_EX, '', cookieOptions)
}
