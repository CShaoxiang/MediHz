interface sliderProp {
  min: number
  max: number
  state: number
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
}

export default function Slider({ min, max, state, onChange }: sliderProp) {
  return (
    <>
      <input
        type="range"
        className="purple-range w-full"
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
