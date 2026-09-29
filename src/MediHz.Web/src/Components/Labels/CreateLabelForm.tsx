import { useState } from 'react'
import Button from '../Common/Button'
import TextInput from '../Common/TextInput'
import Select from '../Common/Select'
import type { Label, LabelGroup } from '../../Types/labels'
import { createLabelInGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'

interface IProps {
  groupId: string | null
  labelGroups: LabelGroup[]
  onSubmit: (label: Label) => void
}

export default function CreateLabelForm({ groupId, labelGroups, onSubmit }: IProps) {
  const [name, setName] = useState('')
  const [targetGroupId, setTargetGroupId] = useState<string | null>(groupId ?? null)

  const handleSubmit = async () => {
    if (name.trim() === '') {
      alert('Invalid label name')
      return
    }

    createLabelInGroup(targetGroupId, name)
      .then((result) => {
        if (result.ok) {
          onSubmit(result.value)
          setName('')
        } else showApiError(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <div className="flex w-full flex-col gap-2">
      <label htmlFor="label-name" className="text-sm font-medium text-gray-700">
        Label name
      </label>

      <TextInput
        id="label-name"
        type="text"
        placeholder="Label name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === 'Enter') handleSubmit()
        }}
      />

      <label htmlFor="label-group" className="text-sm font-medium text-gray-700">
        Label group
      </label>

      <Select
        id="label-group"
        value={targetGroupId ?? ''}
        onChange={(e) => setTargetGroupId(e.target.value || null)}
      >
        <option value="">Unassigned</option>
        {labelGroups.map((group) => (
          <option key={group.id} value={group.id}>
            {group.name}
          </option>
        ))}
      </Select>

      <Button label="Add" onClick={handleSubmit} />
    </div>
  )
}
