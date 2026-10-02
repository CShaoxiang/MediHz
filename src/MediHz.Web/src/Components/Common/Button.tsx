import type { ReactNode } from 'react'

export interface ButtonProps {
  label?: string
  onClick: () => void
  children?: ReactNode
  border?: boolean
  ariaLabel?: string
  classname?: string
}

export default function Button(props: ButtonProps) {
  const baseClasses =
    'button rounded-full bg-purple-700 p-2 text-sm font-medium text-white transition hover:bg-purple-600 focus:outline-none focus-visible:ring-2 focus-visible:ring-purple-300 focus-visible:ring-offset-2'

  if (props.border) {
    return (
      <button
        type="button"
        aria-label={props.ariaLabel}
        className={`${baseClasses} border-2 border-dotted shadow`}
        onClick={props.onClick}
      >
        {props.label && <div className="px-2">{props.label}</div>}
        {props.children}
      </button>
    )
  }

  return (
    <button
      type="button"
      aria-label={props.ariaLabel}
      className={baseClasses}
      onClick={props.onClick}
    >
      {props.label && <div className="px-2">{props.label}</div>}
      {props.children}
    </button>
  )
}
