import { useState } from 'react'
import Button from '../Common/Button'
import TextInput from '../Common/TextInput'
import type { AudioFile } from '../../Types/audio'

interface IProps {
  audio?: AudioFile
  onSubmit: (audio: AudioFile) => void
}

export default function RenameAudioForm({ audio, onSubmit }: IProps) {
  const [audioName, setAudioName] = useState("")

  const handleSubmit = async () => {
    if (audio) {
      if (audioName?.trim() == '') {
        alert('Invalid project details')
        return
      }

      const renamedAudio: AudioFile = { ...audio, fileName: audioName }

      onSubmit(renamedAudio)
    }
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>Rename Audio</h2>
      <TextInput
        type="text"
        placeholder={audio?.fileName}
        value={audioName}
        onChange={(e) => setAudioName(e.target.value)}
      />
      <Button label="Rename" onClick={handleSubmit} />
    </div>
  )
}
