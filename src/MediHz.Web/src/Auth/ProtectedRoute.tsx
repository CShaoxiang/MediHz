import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from './AuthContext'
/**
 * A wrapper component that restricts route access to authenticated users.
 *
 * - Waits for authentication state initialization (`isReady`).
 * - Redirects unauthenticated users to the `/login` route.
 * - Renders children elements only if a valid user session exists.
 */
export default function ProtectedRoute({ children }: { children: ReactNode }) {
  const { user } = useAuth()

  if (!user) {
    return <Navigate to="/login" replace />
  }
  return children
}
