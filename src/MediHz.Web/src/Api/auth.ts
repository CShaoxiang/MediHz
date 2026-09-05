import type { AuthResponse } from '../Types/auth'
import { apiPostJson } from './helpers'

export async function loginWithGoogle(idToken: string) {
  return await apiPostJson<AuthResponse>('auth/google', { idToken })
}
