import type { BatchAudioFileUploadResult } from '../Types/audio'
import { apiFetch, apiFetchForm, apiGetFile } from './helpers'
import type { ApiResult } from '../Types/api'

const AUDIO_FILE_UPLOAD_API_BASE = 'AudioFileUpload'
const AUDIO_FILE_API_BASE = 'AudioFile'

export async function addAudioFiles(projectId: string, files: File[]) {
  return await apiFetchForm<BatchAudioFileUploadResult>(
    `${AUDIO_FILE_UPLOAD_API_BASE}/${projectId}/batch`,
    { files },
  )
}

export async function getAudioFileUrl(
  projectId: string,
  audioId: string,
): Promise<ApiResult<string>> {
  return await apiGetFile(`${AUDIO_FILE_API_BASE}/${projectId}/${audioId}`)
}

import type { AnnotationDto } from '../Types/audio'

export async function getAnnotations(audioId: string) {
  return await apiFetch<AnnotationDto[]>(`${AUDIO_FILE_API_BASE}/${audioId}/annotations`)
}
