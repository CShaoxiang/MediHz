import { useEffect, useState } from 'react'
import type { Label, LabelGroup } from '../../Types/labels'
import { fetchLabelsByGroup, deleteLabelGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'
import CreateLabelForm from './CreateLabelForm'
import LabelItem from './LabelItem'
import { Button } from '../Shadcn/button'
import { DialogFooter, DialogHeader, DialogTitle } from '../Shadcn/dialog'
import ConfirmDeleteDialog from '../Common/ConfirmDeleteDialog'
import EmptyState from '../Common/EmptyState'
import { LuTag } from 'react-icons/lu'

interface IProps {
  groupId: string | null
  groupName: string
  allGroups: LabelGroup[]
  onGroupDeleted: () => void
}

export default function LabelGroupView({ groupId, groupName, allGroups, onGroupDeleted }: IProps) {
  const [labels, setLabels] = useState<Label[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [isConfirmOpen, setIsConfirmOpen] = useState(false)

  useEffect(() => {
    fetchLabelsByGroup(groupId)
      .then((result) => {
        if (result.ok) setLabels(result.value)
        else showApiError(result.error)
      })
      .catch(console.error)
      .finally(() => setIsLoading(false))
  }, [groupId])

  const handleDelete = (id: string) => {
    setLabels((prev) => prev.filter((l) => l.id !== id))
  }

  const handleMove = (labelId: string, newGroupId: string | null) => {
    setLabels((prev) =>
      newGroupId === groupId
        ? prev.map((l) => (l.id === labelId ? { ...l, labelGroupId: newGroupId } : l))
        : prev.filter((l) => l.id !== labelId),
    )
  }

  const handleCreate = (createdLabel: Label) => {
    if (createdLabel.labelGroupId === groupId) {
      setLabels((prev) => [createdLabel, ...prev])
    }
  }

  const handleDeleteGroup = async () => {
    if (!groupId) return
    setIsConfirmOpen(false)
    const result = await deleteLabelGroup(groupId)
    if (result.ok) onGroupDeleted()
    else showApiError(result.error)
  }

  return (
    <>
      <DialogHeader>
        <DialogTitle>{groupName}</DialogTitle>
      </DialogHeader>
      {!isLoading && labels.length === 0 ? (
        <EmptyState
          icon={<LuTag />}
          title="No labels yet"
          description="Add a label below to start building this group."
        />
      ) : (
        <div className="max-h-[50vh] space-y-2 overflow-y-auto">
          {labels.map((label) => (
            <LabelItem
              key={label.id}
              label={label}
              allGroups={allGroups}
              onDelete={handleDelete}
              onMove={handleMove}
            />
          ))}
        </div>
      )}
      <CreateLabelForm groupId={groupId} labelGroups={allGroups} onSubmit={handleCreate} />
      {groupId && (
        <DialogFooter>
          <Button variant="destructive" onClick={() => setIsConfirmOpen(true)}>
            Delete group
          </Button>
        </DialogFooter>
      )}
      <ConfirmDeleteDialog
        open={isConfirmOpen}
        onOpenChange={setIsConfirmOpen}
        title="Delete Label Group"
        description={`Delete "${groupName}" and all its labels?`}
        onConfirm={handleDeleteGroup}
      />
    </>
  )
}
