import { createContext, useContext } from 'react'
import type { User } from '../Types/auth'

export type AuthContextValue = {
  user: User | null
  login: (idToken: string) => Promise<void>
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider.')
  }

  return context
}
