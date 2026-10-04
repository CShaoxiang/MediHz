import Button from '../Common/Button'
import type { AudioFile } from '../../Types/audio'

interface IProps {
  audio?: AudioFile
  onSubmit: (audio: AudioFile) => void
}

export default function DeleteAudioForm({ audio, onSubmit }: IProps) {
  const handleSubmit = async () => {
    if (audio) {
      onSubmit(audio)
    }
  }

  return (
    <div className="flex flex-col gap-2">
      <h2>Delete Audio</h2>
      <p>Do you want to delete {audio?.fileName}?</p>
      <Button label="Delete" onClick={handleSubmit} />
    </div>
  )
}
