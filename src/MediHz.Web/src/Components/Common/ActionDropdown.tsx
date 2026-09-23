import { useState, type ReactNode } from 'react'
import { type ButtonProps } from './Button'

interface DropdownProp {
  label?: string
  children?: ReactNode
  buttons?: ButtonProps[]
}

export default function ActionDropdown({children, buttons = [] }: DropdownProp) {
  const [isOpen, setIsOpen] = useState(false)

  return (
    <div className="rounded-full p-3 ml-auto bg-gray-100 more-button ">
        <button onClick={(e) => {e.stopPropagation(); setIsOpen(!isOpen)}}>{children} </button>

      {isOpen && buttons.length > 0 && (
        <div className="absolute left-0 top-full z-50 flex bg-gray-100 min-w-full flex-col p-2">
          {buttons.map((button) => (
            <button
              key={button.label}
              type="button"
              className="more-button"
              onClick={(e) => {e.stopPropagation(); button.onClick(e)}}
            >
              {button.label}
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
