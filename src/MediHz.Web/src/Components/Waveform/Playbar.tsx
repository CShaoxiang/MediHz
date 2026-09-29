import Slider from '../Common/Slider'

interface PlaybarProp {
  audioDuration: number
  state: number
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
}

export default function Playbar({ audioDuration, state, onChange }: PlaybarProp) {
  return (
    <Slider
      min={0}
      max={audioDuration ?? 0}
      state={state}
      onChange={onChange}
      ariaLabel="Audio playback position"
    />
  )
}
