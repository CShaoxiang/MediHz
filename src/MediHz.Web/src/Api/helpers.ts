import type { ApiResult, ProblemDetails } from '../Types/api'
import { getToken } from '../Auth/tokenStorage'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

const API_BASE = '/api'
const DEFAULT_PROBLEM: ProblemDetails = {
  title: 'An unexpected error occurred.',
  detail: 'No details available.',
  status: 500,
}

async function request<T>(
  path: string,
  method: HttpMethod,
  extraHeaders: HeadersInit = {},
  body?: BodyInit,
): Promise<ApiResult<T>> {
  console.info('Fetching data', { method, path })

  const token = getToken()
  const headers = {
    Accept: 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...extraHeaders,
  }

  const response = await fetch(`${API_BASE}/${path}`, { method, headers, body })

  console.info('Response received', { status: response.status, statusText: response.statusText })

  try {
    const result = await response.json()
    return response.ok ? { ok: true, value: result } : { ok: false, error: result }
  } catch {
    if (response.ok) return { ok: true, value: null as T }
    return { ok: false, error: DEFAULT_PROBLEM }
  }
}

export async function apiFetch<T = unknown>(
  path: string,
  body?: Record<string, unknown>,
  method: HttpMethod = 'GET',
): Promise<ApiResult<T>> {
  return request<T>(
    path,
    method,
    { 'Content-Type': 'application/json' },
    body ? JSON.stringify(body) : undefined,
  )
}

export async function apiFetchForm<T = unknown>(
  path: string,
  body: Record<string, unknown>,
  method: HttpMethod = 'POST',
): Promise<ApiResult<T>> {
  return request<T>(path, method, {}, toFormData(body))
}

export async function apiPatch<T = unknown>(
  path: string,
  body?: Record<string,unknown>,
  method: HttpMethod = 'PATCH',
): Promise<ApiResult<T>> {
  return request<T>(
    path,method,{'Content-Type': 'application/json'},body? JSON.stringify(body) : undefined,
  )
}

export async function apiDelete<T = unknown>(path: string,
  body?: Record<string, unknown>,
  method: HttpMethod = 'DELETE',
): Promise<ApiResult<T>> {
  return request<T>(
    path,
    method,
    { 'Content-Type': 'application/json' },
    body ? JSON.stringify(body) : undefined,
  )
}

export function showApiError(error: ProblemDetails) {
  console.error(error)
  alert(error.detail || error.title)
}

export async function apiGetFile(path: string): Promise<ApiResult<string>> {
  console.info('Fetching file with GET', { path })
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'GET',
    headers: {
      Accept: '*/*',
      Authorization: `Bearer ${getToken()}`,
    },
  })

  console.info('Response received', { status: response.status, statusText: response.statusText })

  if (!response.ok) {
    const error = await response.json()
    return { ok: false, error: error }
  }

  const blob = await response.blob()
  const url = URL.createObjectURL(blob)

  return { ok: true, value: url }
}

function toFormData(fields: Record<string, unknown>): FormData {
  const form = new FormData()
  for (const [key, value] of Object.entries(fields)) {
    if (value == null) continue
    for (const v of Array.isArray(value) ? value : [value]) {
      if (v == null) continue
      form.append(key, v instanceof Blob ? v : String(v))
    }
  }
  return form
}
