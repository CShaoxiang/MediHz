import type { AuthResponse } from '../Types/auth'
import { apiFetch } from './helpers'

export async function loginWithGoogle(idToken: string) {
  return await apiFetch<AuthResponse>('auth/google', { idToken }, 'POST')
}
