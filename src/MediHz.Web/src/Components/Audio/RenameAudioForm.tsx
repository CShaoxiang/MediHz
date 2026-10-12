import { useState, type FormEvent } from 'react'
import { Button } from '../Shadcn/button'
import { DialogFooter, DialogHeader, DialogTitle } from '../Shadcn/dialog'
import { Field, FieldError, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import type { AudioFile } from '../../Types/audio'

interface IProps {
  audio?: AudioFile
  onSubmit: (audio: AudioFile) => void
}

export default function RenameAudioForm({ audio, onSubmit }: IProps) {
  const [audioName, setAudioName] = useState(audio?.fileName ?? '')
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (!audio) return
    if (audioName.trim() === '') {
      setError('File name is required')
      return
    }
    onSubmit({ ...audio, fileName: audioName })
  }

  return (
    <form onSubmit={handleSubmit} className="contents">
      <DialogHeader>
        <DialogTitle>Rename Audio</DialogTitle>
      </DialogHeader>
      <Field data-invalid={!!error}>
        <FieldLabel htmlFor="audio-rename">File name</FieldLabel>
        <Input
          id="audio-rename"
          value={audioName}
          aria-invalid={!!error}
          onChange={(e) => {
            setAudioName(e.target.value)
            setError(null)
          }}
        />
        <FieldError>{error}</FieldError>
      </Field>
      <DialogFooter>
        <Button type="submit">Rename</Button>
      </DialogFooter>
    </form>
  )
}
