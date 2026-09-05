import type { ApiResult } from '../Types/api'

const API_BASE = '/api'

export async function apiGet<T = unknown>(path: string): Promise<ApiResult<T>> {
  console.info('Fetching data with GET', { path })
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'GET',
    headers: {
      Accept: 'application/json',
    },
  })

  console.info('Response received', { status: response.status, statusText: response.statusText })

  const result = await response.json()
  return response.ok ? { ok: true, value: result } : { ok: false, error: result }
}

export async function apiPostForm<T = unknown>(
  path: string,
  body: Record<string, unknown>,
): Promise<ApiResult<T>> {
  console.info('Sending data with POST', { path, body })
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'POST',
    headers: {
      Accept: 'application/json',
    },
    body: toFormData(body),
  })

  console.info('Response received', { status: response.status, statusText: response.statusText })

  const result = await response.json()
  return response.ok ? { ok: true, value: result } : { ok: false, error: result }
}

export async function apiPostJson<T = unknown>(
  path: string,
  body: Record<string, unknown>,
): Promise<ApiResult<T>> {
  console.info('Sending data with POST', { path, body })
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  console.info('Response received', { status: response.status, statusText: response.statusText })

  const result = await response.json()
  return response.ok ? { ok: true, value: result } : { ok: false, error: result }
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
