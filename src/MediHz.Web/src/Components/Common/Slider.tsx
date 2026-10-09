interface SliderProp {
  min: number
  max: number
  state: number
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void
  ariaLabel: string
  step?: number
}

export default function Slider({ min, max, state, onChange, ariaLabel, step = 1 }: SliderProp) {

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') {
          e.preventDefault()
          if (e.key === 'ArrowLeft') {
            onChange({
              target: { value: Math.max(state - 1, min).toString() },
            } as React.ChangeEvent<HTMLInputElement>)
          }else{
            onChange({
              target: { value: Math.max(state + 1, min).toString() },
            } as React.ChangeEvent<HTMLInputElement>)
          }
        }}

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
      onKeyDown={handleKeyDown}
    />
  )
}
