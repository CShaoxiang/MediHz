import type { User } from '../Types/auth'

const tokenKey = 'medihz.accessToken'
const userKey = 'medihz.user'

/** store authentication session into browser session storage*/
export function saveSession(accessToken: string, user: User) {
  sessionStorage.setItem(tokenKey, accessToken)
  sessionStorage.setItem(userKey, JSON.stringify(user))
}

export function getToken(): string | null {
  return sessionStorage.getItem(tokenKey)
}

/**Retrieves and parses the user profile from storage , Automatically clears the session */
export function getStoredUser(): User | null {
  const rawUser = sessionStorage.getItem(userKey)

  if (!rawUser) return null

  try {
    return JSON.parse(rawUser) as User
  } catch {
    clearSession()
    return null
  }
}

export function clearSession() {
  sessionStorage.removeItem(tokenKey)
  sessionStorage.removeItem(userKey)
}
