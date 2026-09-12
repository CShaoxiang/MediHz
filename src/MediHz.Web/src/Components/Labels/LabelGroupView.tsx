import { useEffect, useState } from 'react'
import type { Label, LabelGroup } from '../../Types/labels'
import { fetchLabelsByGroup, deleteLabelGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'
import CreateLabelForm from './CreateLabelForm'
import LabelItem from './LabelItem'
import Button from '../Common/Button'

interface IProps {
  groupId: string | null
  groupName: string
  allGroups: LabelGroup[]
  onClose: () => void
}

export default function LabelGroupView({ groupId, groupName, allGroups, onClose }: IProps) {
  const [labels, setLabels] = useState<Label[]>([])

  useEffect(() => {
    fetchLabelsByGroup(groupId)
      .then((result) => {
        if (result.ok) setLabels(result.value)
        else showApiError(result.error)
      })
      .catch(console.error)
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
    if (!groupId || !confirm(`Delete "${groupName}" and all its labels?`)) return
    const result = await deleteLabelGroup(groupId)
    if (result.ok) onClose()
    else showApiError(result.error)
  }

  return (
    <div className="flex flex-col gap-4">
      {groupId && <Button label="Delete group" onClick={handleDeleteGroup} />}
      <div className="space-y-2">
        {labels.map((label) => (
          <LabelItem
            key={label.id}
            label={label}
            allGroups={allGroups}
            onDelete={handleDelete}
            onMove={handleMove}
          />
        ))}
        <CreateLabelForm groupId={groupId} labelGroups={allGroups} onSubmit={handleCreate} />
      </div>
    </div>
  )
}
