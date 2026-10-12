export interface MenuAction {
  label: string
  onClick: () => void
  variant?: 'default' | 'destructive'
}
