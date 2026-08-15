const API_BASE = "/api";

export async function apiGet<T = unknown>(
  path: string,
): Promise<T> {
  console.info("Fetching data with GET", { path });
  const response = await fetch(`${API_BASE}/${path}`, {
    method: "GET",
    headers: {
      Accept: "application/json",
    },
  });

  console.info("Response received", { status: response.status, statusText: response.statusText });
  
  if (!response.ok) {
    throw new Error(`Failed to fetch data: ${response.statusText}`);
  }

  const text = await response.text();
  const json = await JSON.parse(text);

  return json;
}

export async function apiPostForm<T = unknown>(
  path: string,
  body: Record<string, unknown>,
): Promise<T> {
  console.info("Sending data with POST", { path, body });
  const response = await fetch(`${API_BASE}/${path}`, {
    method: "POST",
    headers: {
      Accept: "application/json",
    },
    body: toFormData(body)
  });

  console.info("Response received", { status: response.status, statusText: response.statusText });
  
  if (!response.ok) {
    throw new Error(`Failed to send data: ${response.statusText}`);
  }

  const text = await response.text();
  const json = await JSON.parse(text);

  return json;
}

function toFormData(fields: Record<string, unknown>): FormData {
  const form = new FormData();
  for (const [key, value] of Object.entries(fields)) {
    if (value == null) continue;
    for (const v of Array.isArray(value) ? value : [value]) {
      if (v == null) continue;
      form.append(key, v instanceof Blob ? v : String(v));
    }
  }
  return form;
}
