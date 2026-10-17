export interface AnnotationRegion {
  id: string
  saved: boolean
  changed: boolean
  startTime: number
  endTime: number
  tags: Set<string>
  authorId?: string
  authorDisplayName?: string
}
