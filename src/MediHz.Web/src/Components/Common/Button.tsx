import type { ReactNode } from 'react'

export interface ButtonProps {
  label?: string
  onClick?: (args: any) => void
  children?: ReactNode
}

export default function Button(props: ButtonProps) {
  return (
    <button
      className="button roundered-3xl bg-purple-700 p-2 text-left text-sm font-medium text-white transition hover:bg-purple-600"
      onClick={props.onClick}
    >
      {props.label && <div className="px-2">{props.label}</div>}

      {props.children}
    </button>
  )
}
