import type { AuthResponse } from '../Types/auth'
import { apiPostJson } from './helpers'

export function loginWithGoogle(idToken: string): Promise<AuthResponse> {
  return apiPostJson<AuthResponse>('auth/google', { idToken })
}
