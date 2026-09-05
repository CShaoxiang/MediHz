import type { AudioFile } from '../Types/audio'
import type { OmitId } from '../Types/helpers'
import type { Project } from '../Types/projects'
import { apiGet, apiPostForm } from './helpers'

const PROJECTS_API_BASE = 'Projects'

export async function fetchProjects() {
  console.info('Fetching all projects')
  return await apiGet<Project[]>(`${PROJECTS_API_BASE}/all`)
}

export async function fetchProjectAudioFiles(projectId: string) {
  console.info('Fetching project audio files:', projectId)
  return await apiGet<AudioFile[]>(`${PROJECTS_API_BASE}/${projectId}/files`)
}

export async function createProject(project: OmitId<Project>) {
  console.info('Creating new project:', project)
  return await apiPostForm<Project>(`${PROJECTS_API_BASE}/create`, project)
}
