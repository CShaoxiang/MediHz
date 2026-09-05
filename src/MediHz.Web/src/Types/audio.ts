import type { ApiStatus } from './api'

export type AudioFile = {
  id: string
  fileName: string
}

export type AudioFileUploadResult = {
  id: string
  fileName: string
  mimeType: string | null
  sizeBytes: number
  status: (typeof ApiStatus)[keyof typeof ApiStatus]
  errorMessage: string | null
}

export type BatchAudioFileUploadResult = {
  batchId: string
  items: AudioFileUploadResult[]
}
