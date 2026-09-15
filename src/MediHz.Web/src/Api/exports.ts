import type { FileExportDto, ProjectExportDto } from '../Types/exports'
import { apiFetch } from './helpers'

const EXPORTS_API_BASE = 'Exports'

export async function exportProject(projectId: string) {
  console.info('Exporting project:', projectId)
  return await apiFetch<ProjectExportDto>(`${EXPORTS_API_BASE}/project/${projectId}`)
}

export async function exportFile(fileId: string) {
  console.info('Exporting file:', fileId)
  return await apiFetch<FileExportDto>(`${EXPORTS_API_BASE}/file/${fileId}`)
}
