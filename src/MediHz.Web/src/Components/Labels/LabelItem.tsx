import Button from '../Common/Button'
import Select from '../Common/Select'
import type { Label, LabelGroup } from '../../Types/labels'
import { deleteLabel, moveLabelToGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'

interface IProps {
  label: Label
  allGroups: LabelGroup[]
  onDelete: (id: string) => void
  onMove: (labelId: string, groupId: string | null) => void
}

export default function LabelItem({ label, allGroups, onDelete, onMove }: IProps) {
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
    <div className="flex flex-col gap-2 rounded-lg border border-gray-200 p-3">
      <span className="font-medium break-words">{label.name}</span>
      <div className="flex items-center justify-between gap-2">
        <Select
          aria-label={`Move ${label.name} to label group`}
          value={label.labelGroupId ?? ''}
          onChange={(e) => handleMove(e.target.value || null)}
          className="min-w-0 flex-1"
        >
          <option value="">Unassigned</option>
          {allGroups.map((g) => (
            <option key={g.id} value={g.id}>
              {g.name}
            </option>
          ))}
        </Select>
        <Button label="Delete" onClick={handleDelete} />
      </div>
    </div>
  )
}
