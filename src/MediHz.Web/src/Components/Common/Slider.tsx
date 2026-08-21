import type { Dispatch, SetStateAction } from 'react'

interface sliderProp {
  min: number
  max: number
  state: number
  onChange: (event: any) => void
}

export default function Slider({ min, max, state, onChange }: sliderProp) {
  return (
    <>
      <input
        type="range"
        className="form-range purple-range"
        min={min}
        max={max}
        step="0.001"
        value={state}
        onChange={onChange}
        id="Hello"
      />
    </>
  )
}
