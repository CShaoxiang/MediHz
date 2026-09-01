export type User = {
  userId: string
  email: string
  displayName: string
  googleSubject: string
  createdAt: string
}

export type AuthResponse = {
  accessToken: string
  expiresIn: string
  user: User
}
