import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { loginWithGoogle } from '../Api/auth'
import type { User } from '../Types/auth'
import {
  clearSession,
  getStoredUser,
  getToken,
  saveSession,
} from './tokenStorage'

type AuthContextValue = {
  user: User | null
  isReady: boolean
  login: (idToken: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

/**Context provider that initializes and broadcasts the current authentication state.*/
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [isReady, setIsReady] = useState(false)

  useEffect(() => {
    const token = getToken()
    const storedUser = getStoredUser()

    if (token && storedUser) {
      setUser(storedUser)
    } else {
      clearSession()
    }

    setIsReady(true)
  }, [])

  async function login(idToken: string) {
    const response = await loginWithGoogle(idToken)
    saveSession(response.accessToken, response.user)
    setUser(response.user)
  }

  function logout() {
    clearSession()
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, isReady, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}
/**Custom hook to safely consume authentication state and methods.*/
export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider.')
  }

  return context
}
