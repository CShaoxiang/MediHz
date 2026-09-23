import type { ReactNode } from 'react'

export interface ButtonProps {
  label?: string
  onClick: (e: any) => void
  children?: ReactNode
  border?: boolean
}

export default function Button(props: ButtonProps) {
  if (props.border) {
    return (
      <button
        className="button border-dotted border-2 roundered-full shadow bg-purple-700 p-2 text-sm font-medium text-white  transition hover:bg-purple-600"
        onClick={props.onClick}
      >
        {props.label && <div className="px-2">{props.label}</div>}

        {props.children}
      </button>
    )
  }

  return (
    <button
      className="button roundered-full bg-purple-700 p-2 text-sm font-medium text-white  transition hover:bg-purple-600"
      onClick={props.onClick}
    >
      {props.label && <div className="px-2">{props.label}</div>}

      {props.children}
    </button>
  )
}
