import { GoogleLogin } from '@react-oauth/google'
import { Navigate, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { useAuth } from '../Auth/AuthContext'

export default function LoginPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const [error, setError] = useState<string | null>(null)

  if (user) {
    return <Navigate to="/projects" replace />
  }

  return (
    <main className="flex min-h-screen items-center justify-center bg-purple-50 px-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 text-center shadow-xl">
        <div className="mb-6">
          <h1 className="text-4xl font-bold text-purple-700">MediHz</h1>
          <p className="mt-2 text-gray-600">Medical Audio Annotation Platform</p>
        </div>

        <div className="mb-6">
          <h2 className="text-2xl font-semibold text-gray-800">Welcome</h2>
          <p className="mt-2 text-sm text-gray-500">
            Sign in to access your projects and continue annotating medical audio.
          </p>
        </div>

        {error && (
          <div role="alert" className="mb-4 rounded-lg bg-red-50 p-3 text-sm text-red-700">
            {error}
          </div>
        )}

        <div className="flex justify-center">
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
                setError('Unable to login. Please try again.')
              }
            }}
            onError={() => {
              setError('Google sign-in failed. Please try again.')
            }}
          />
        </div>

        <p className="mt-6 text-xs text-gray-400">
          Secure access to the MediHz annotation platform
        </p>
      </div>
    </main>
  )
}
