import type { ApiResult } from "../Types/api";

const API_BASE = "/api";
import { getToken } from '../Auth/tokenStorage'


function authHeaders(extra: HeadersInit = {}): HeadersInit {
  const token = getToken()

  return {
    Accept: 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...extra,
  }
}

async function readResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`)
  }

  return response.json() as Promise<T>
}

export async function apiGet<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE}/${path}`, {
    headers: authHeaders(),
  })

  return readResponse<T>(response)
}

export async function apiPostJson<T>(path: string, body: unknown): Promise<T> {
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'POST',
    headers: authHeaders({
      'Content-Type': 'application/json',
    }),
    body: JSON.stringify(body),
  })

  return readResponse<T>(response)
}

export async function apiPostForm<T>(
  path: string,
  body: Record<string, unknown>,
): Promise<ApiResult<T>> {
  console.info("Sending data with POST", { path, body });
  const response = await fetch(`${API_BASE}/${path}`, {
    method: 'POST',
    headers: authHeaders(),
    body: toFormData(body),
  })

  return readResponse<T>(response)
  console.info("Response received", { status: response.status, statusText: response.statusText });
  
  const result = await response.json();
  return response.ok 
      ? { ok: true, value: result } 
      : { ok: false, error: result };
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
