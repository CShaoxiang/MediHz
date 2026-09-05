export type ProblemDetails = {
  title: string
  detail: string
  status: number
}

export type ApiResult<T> = { ok: true; value: T } | { ok: false; error: ProblemDetails }

export const ApiStatus = {
  Failure: 0,
  Success: 1,
} as const
