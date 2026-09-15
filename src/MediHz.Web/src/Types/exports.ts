export interface AnnotationExportDto {
  annotation_id: string
  label: string
  start_time: number
  end_time: number
  annotator_id: string
}

export interface FileExportDto {
  file_id: string
  file_name: string
  file_type?: string | null
  duration_seconds: number
  sample_rate: number
  channels?: number | null
  annotations: AnnotationExportDto[]
}

export interface ProjectExportDto {
  project_id: string
  project_name: string
  annotator_ids?: string[] | null
  files: FileExportDto[]
}
