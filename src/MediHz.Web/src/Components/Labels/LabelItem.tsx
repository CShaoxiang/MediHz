import { Button } from '../Shadcn/button'
import { Badge } from '../Shadcn/badge'
import LabelGroupSelect from './LabelGroupSelect'
import { Card, CardContent, CardHeader, CardTitle } from '../Shadcn/card'
import type { Label, LabelGroup } from '../../Types/labels'
import { deleteLabel, moveLabelToGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'
import { LuShieldCheck } from 'react-icons/lu'

interface IProps {
  label: Label
  allGroups: LabelGroup[]
  onDelete: (id: string) => void
  onMove: (labelId: string, groupId: string | null) => void
}

export default function LabelItem({ label, allGroups, onDelete, onMove }: IProps) {
  const isPrePackaged = label.ownerId === null

  const handleDelete = async () => {
    const result = await deleteLabel(label.id)
    if (result.ok) onDelete(label.id)
    else showApiError(result.error)
  }

  const handleMove = async (groupId: string | null) => {
    const result = await moveLabelToGroup(label.id, groupId)
    if (result.ok) {
      onMove(label.id, groupId)
    } else showApiError(result.error)
  }

  return (
    <Card size="sm">
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className="break-words">{label.name}</CardTitle>
        {isPrePackaged && (
          <Badge variant="outline" className="flex items-center gap-1 text-xs">
            <LuShieldCheck className="size-3 text-primary" />
            Pre-packaged
          </Badge>
        )}
      </CardHeader>
      <CardContent className="flex items-center gap-2">
        <LabelGroupSelect
          aria-label={`Move ${label.name} to label group`}
          className="min-w-0 flex-1"
          groups={allGroups}
          value={label.labelGroupId}
          onValueChange={handleMove}
          disabled={isPrePackaged}
        />
        {!isPrePackaged && (
          <Button variant="destructive" onClick={handleDelete}>
            Delete
          </Button>
        )}
      </CardContent>
    </Card>
  )
}
