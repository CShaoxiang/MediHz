import { GoogleLogin } from '@react-oauth/google'
import { Navigate, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { useAuth } from '../Auth/AuthProvider'

export default function LoginPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const [error, setError] = useState<string | null>(null)

  if (user) {
    return <Navigate to="/projects" replace />
  }

  return (
    <main>
      <h1>Sign in to MediHz</h1>

      {error && <p role="alert">{error}</p>}
      <GoogleLogin
        onSuccess={async (response) => {
          if (!response.credential) {
            setError('Google sign-in did not return a credential')
            return
          }

          try {
            setError(null)
            await login(response.credential)
            navigate('/projects', { replace: true })
          } catch {
            setError('Unable to login')
          }
        }}
        onError={() => {
          setError('Google sign-in failed')
        }}
      />
    </main>
  )
}
