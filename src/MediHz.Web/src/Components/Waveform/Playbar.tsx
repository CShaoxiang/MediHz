import type { Dispatch, RefObject, SetStateAction } from "react"
import Slider from "../Common/Slider"

interface PlaybarProp{
    audioDuration: RefObject<number|null>,
    state: number,
    onChange:  Dispatch<SetStateAction<number>>
}

export default function Playbar({audioDuration, state, onChange}:PlaybarProp) {
  return (
    <Slider
      min={0}
      max={audioDuration.current ? audioDuration.current : 0}
      state={state}
      onChange={onChange}
    />
  )
}
