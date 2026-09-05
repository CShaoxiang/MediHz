import type { BatchAudioFileUploadResult } from '../Types/audio'
import { apiPostForm } from './helpers'

const AUDIO_FILE_UPLOAD_API_BASE = 'AudioFileUpload'

export async function addAudioFiles(projectId: string, files: File[]) {
  return await apiPostForm<BatchAudioFileUploadResult>(
    `${AUDIO_FILE_UPLOAD_API_BASE}/${projectId}/batch`,
    { files },
  )
}
