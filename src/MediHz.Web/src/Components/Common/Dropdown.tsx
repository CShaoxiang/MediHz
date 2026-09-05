import { useState } from 'react'
import { MdOutlineKeyboardArrowDown, MdOutlineKeyboardArrowUp } from 'react-icons/md'
import Button, { type ButtonProps } from './Button'

interface DropdownProp {
  label: string
  buttons?: ButtonProps[]
}

export default function Dropdown({ label = '', buttons = [] }: DropdownProp) {
  const [isOpen, setIsOpen] = useState(false)

  return (
    <div className="relative w-fit rounded-3xl border-3 border-purple-700 bg-purple-100">
      <span className="inline-flex items-center gap-2 rounded-3xl bg-purple-700">
        <Button label={label} onClick={() => setIsOpen(!isOpen)} />
        <span className="flex items-center justify-center text-white">
          {isOpen ? (
            <MdOutlineKeyboardArrowDown size={25} />
          ) : (
            <MdOutlineKeyboardArrowUp size={25} />
          )}
        </span>
      </span>

      {isOpen && buttons.length > 0 && (
        <div className="absolute left-0 top-full z-50 flex min-w-full flex-col gap-2 rounded-3xl border-3 border-purple-700 bg-purple-100 p-2">
          {buttons.map((button) => (
            <button
              key={button.label}
              type="button"
              className="button bg-purple-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-purple-600"
              onClick={() => button.onClick?.()}
            >
              {button.label}
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
