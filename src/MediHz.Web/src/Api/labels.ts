import type { Label, LabelGroup } from '../Types/labels'
import { apiFetch, apiFetchForm } from './helpers'

const LABELS_API_BASE = 'Labels'

export async function fetchLabelGroups() {
  console.info('Fetching all label groups')
  return await apiFetch<LabelGroup[]>(`${LABELS_API_BASE}/groups`)
}

export async function fetchAllLabels() {
  console.info('Fetching all labels')
  return await apiFetch<Label[]>(`${LABELS_API_BASE}/all`)
}

export async function createLabelGroup(name: string) {
  console.info('Creating new label group:', name)
  return await apiFetchForm<LabelGroup>(`${LABELS_API_BASE}/groups`, { name })
}

export async function deleteLabelGroup(id: string) {
  console.info('Deleting label group:', id)
  return await apiFetch<void>(`Labels/groups/${id}`, undefined, 'DELETE')
}

export async function fetchLabelsByGroup(groupId: string | null) {
  console.info('Fetching labels for group:', groupId)
  const query = groupId ? `?groupId=${encodeURIComponent(groupId)}` : ''
  return await apiFetch<Label[]>(`${LABELS_API_BASE}${query}`)
}

export async function createLabelInGroup(groupId: string | null, name: string) {
  console.info('Creating new label in group:', { groupId, name })
  return await apiFetchForm<Label>(LABELS_API_BASE, { name, groupId })
}

export async function deleteLabel(id: string) {
  console.info('Deleting label:', id)
  return await apiFetch<void>(`Labels/${id}`, undefined, 'DELETE')
}

export async function moveLabelToGroup(labelId: string, targetGroupId: string | null) {
  console.info('Moving label to group:', { labelId, targetGroupId })
  return await apiFetchForm<Label>(`Labels/${labelId}`, { targetGroupId }, 'PATCH')
}
