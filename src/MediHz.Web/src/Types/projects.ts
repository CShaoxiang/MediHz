export type Project = {
  id: string
  name: string
  ownerId: string
  role: ProjectRole
}

export type ProjectRole = 'Owner' | 'Annotator' | 'Viewer'
export type AddMemberRole = Exclude<ProjectRole, 'Owner'>
export type ProjectMember = {
  projectId: string
  userId: string
  email: string
  displayName: string
  role: ProjectRole
  joinedAt: string
}
