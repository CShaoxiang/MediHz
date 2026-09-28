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

export type AudioFileUrlResult = {
  audioId: string
  audioPath: URL
}

export type AnnotationDto = {
  id: string
  audioFileId: string
  authorId: string
  authorDisplayName: string
  labelNames: string[]
  startTimeSeconds: number
  endTimeSeconds: number
  durationSeconds: number
  createdAt: string
  updatedAt: string
}
