import { getStoredUser } from '../Auth/tokenStorage'
import type { AudioFile } from '../Types/audio'
import type { Project, ProjectMember, AddMemberRole } from '../Types/projects'
import { apiDelete, apiFetch, apiFetchForm, apiPatch } from './helpers'

const PROJECTS_API_BASE = 'Projects'

export function fetchProject(projectId: string) {
  return apiFetch<Project>(`${PROJECTS_API_BASE}/${projectId}`)
}

export function addProjectMember(projectId: string, email: string, role: AddMemberRole) {
  return apiFetch<ProjectMember>(
    `${PROJECTS_API_BASE}/${projectId}/members`,
    { email, role },
    'POST',
  )
}

export async function fetchProjects() {
  console.info('Fetching all projects')
  return await apiFetch<Project[]>(`${PROJECTS_API_BASE}/all`)
}

export async function fetchProjectAudioFiles(projectId: string) {
  console.info('Fetching project audio files:', projectId)
  return await apiFetch<AudioFile[]>(`${PROJECTS_API_BASE}/${projectId}/files`)
}

export async function createProject(project: Pick<Project, 'name'>) {
  console.info('Creating new project:', project)
  return await apiFetchForm<Project>(`${PROJECTS_API_BASE}/create`, project)
}

export async function fetchSingleAudio(projectId: string, audioId: string) {
  console.info('Fetching single audio file', audioId)
  return await apiFetch<AudioFile>(`${PROJECTS_API_BASE}/${projectId}/${audioId}`)
}

export async function renameProject(projectId: string, newName: string) {
  console.info('Renaming selected project to:', newName)
  return await apiPatch<Project>(`${PROJECTS_API_BASE}/${projectId}/${getStoredUser()?.userId}`, {
    name: newName,
  })
}

export async function deleteProject(projectId: string) {
  console.info('Deleting selected project:', projectId)
  return await apiDelete(`${PROJECTS_API_BASE}/${projectId}/${getStoredUser()?.userId}`)
}
