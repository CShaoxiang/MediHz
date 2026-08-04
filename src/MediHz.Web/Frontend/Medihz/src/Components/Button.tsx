export interface ButtonProps {
  label: string
  onClick?: (item: string) => void
}

export default function Button(props: ButtonProps) {
  return (
    <button
      className="button roundered-3xl bg-purple-700 px-4 py-2 text-left text-sm font-medium text-white transition hover:bg-purple-600"
      onClick={() => props.onClick?.(props.label)}
    >
      {props.label}
    </button>
  )
}
