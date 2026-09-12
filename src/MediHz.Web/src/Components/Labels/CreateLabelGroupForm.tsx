import { useState } from 'react'
import Button from '../Common/Button'
import TextInput from '../Common/TextInput'
import type { LabelGroup } from '../../Types/labels'
import { createLabelGroup } from '../../Api/labels'
import { showApiError } from '../../Api/helpers'

interface IProps {
  onSubmit: (labelGroup: LabelGroup) => void
}

export default function CreateLabelGroupForm({ onSubmit }: IProps) {
  const [name, setName] = useState('')

  const handleSubmit = async () => {
    if (name.trim() === '') {
      alert('Invalid label group name')
      return
    }

    createLabelGroup(name)
      .then((result) => {
        if (result.ok) {
          onSubmit(result.value)
          setName('')
        } else showApiError(result.error)
      })
      .catch((e) => console.error(e))
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>Create a New Label Group</h2>
      <TextInput
        type="text"
        placeholder="Group name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === 'Enter') handleSubmit()
        }}
      />
      <Button label="Create +" onClick={handleSubmit} />
    </div>
  )
}
