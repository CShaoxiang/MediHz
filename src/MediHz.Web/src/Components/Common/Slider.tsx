interface SliderProp {
  min: number
  max: number
  state: number
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
  ariaLabel: string
  step?: number
}

export default function Slider({ min, max, state, onChange, ariaLabel, step = 1 }: SliderProp) {
  return (
    <input
      type="range"
      className="purple-range w-full focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-300 focus-visible:ring-offset-2"
      min={min}
      max={max}
      step={step}
      value={state}
      onChange={onChange}
      aria-label={ariaLabel}
    />
  )
}
