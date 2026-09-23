import type { ReactNode } from 'react'
import type { ButtonProps } from './Button'
import { DropdownMenuBasic } from '../Shadcn/actionDropdownShadcn'
interface CardProps {
  title: string
  onClick: () => void
  children?: ReactNode
  actions?: ButtonProps[]
}

export default function Card({ title, onClick, children, actions }: CardProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="rounded-lg border-2 border-dotted border-medihzprimary p-4 text-left shadow-lg transition-transform duration-300 hover:scale-105 focus:outline-none focus-visible:ring-2 focus-visible:ring-medihzprimary"
    >
      <span className="flex items-center gap-2">
        {children}
        <h5 className="text-lg font-semibold text-gray-800">{title}</h5>
        {<span className='flex justify-end ml-auto'><DropdownMenuBasic/></span>}
      </span>
      
    </button>
  )
}
