import { useState, type ReactNode } from 'react'
import { loginWithGoogle } from '../Api/auth'
import type { User } from '../Types/auth'
import { clearSession, getStoredUser, getToken, saveSession } from './tokenStorage'
import { AuthContext } from './AuthContext'

/**Context provider that initializes and broadcasts the current authentication state.*/
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    const token = getToken()
    const storedUser = getStoredUser()
    if (token && storedUser) return storedUser
    clearSession()
    return null
  })

  async function login(idToken: string) {
    const response = await loginWithGoogle(idToken)
    if (response.ok) {
      const { accessToken, user } = response.value
      saveSession(accessToken, user)
      setUser(user)
    } else {
      alert(response.error.detail)
    }
  }

  function logout() {
    clearSession()
    setUser(null)
  }

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>
}
