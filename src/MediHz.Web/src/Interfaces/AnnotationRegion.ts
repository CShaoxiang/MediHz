export interface AnnotationRegion {
  id: string
  saved: boolean
  startTime: number
  endTime: number
  tags: Set<string>
  authorId?: string
  authorDisplayName?: string
}
