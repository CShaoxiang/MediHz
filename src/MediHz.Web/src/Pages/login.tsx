import { GoogleLogin } from '@react-oauth/google'
import { Navigate, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { useAuth } from '../Auth/AuthContext'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '../Components/Shadcn/card'

export default function LoginPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()

  if (user) {
    return <Navigate to="/projects" replace />
  }

  return (
    <div className="flex h-full items-center justify-center rounded-3xl bg-accent px-4">
      <Card className="w-full max-w-md text-center">
        <CardHeader>
          <CardTitle className="text-4xl font-bold text-primary">MediHz</CardTitle>
          <CardDescription>Medical Audio Annotation Platform</CardDescription>
        </CardHeader>

        <CardContent className="flex flex-col gap-4">
          <div>
            <h2 className="text-2xl font-semibold">Welcome</h2>
            <p className="mt-2 text-muted-foreground">
              Sign in to access your projects and continue annotating medical audio.
            </p>
          </div>

          <div className="flex justify-center">
            <GoogleLogin
              onSuccess={async (response) => {
                if (!response.credential) {
                  toast.error('Google sign-in did not return a credential')
                  return
                }

                try {
                  await login(response.credential)
                  navigate('/projects', { replace: true })
                } catch (error) {
                  toast.error(
                    error instanceof Error && error.message
                      ? error.message
                      : 'Unable to login. Please try again.',
                  )
                }
              }}
              onError={() => {
                toast.error('Google sign-in failed. Please try again.')
              }}
            />
          </div>
        </CardContent>

        <CardFooter className="justify-center text-xs text-muted-foreground">
          Secure access to the MediHz annotation platform
        </CardFooter>
      </Card>
    </div>
  )
}
