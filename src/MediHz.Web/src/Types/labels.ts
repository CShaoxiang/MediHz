export type LabelGroup = {
  id: string
  name: string
  ownerId: string | null
  createdAtUtc: string
}

export type Label = {
  id: string
  name: string
  labelGroupId: string | null
  ownerId: string | null
  createdAtUtc: string
}
