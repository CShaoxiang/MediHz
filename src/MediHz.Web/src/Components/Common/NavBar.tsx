import type { ReactNode } from 'react'
import Button from './Button'

interface NavBarProp {
  children?: ReactNode
}

export default function NavBar({ children }: NavBarProp) {
  return (
    <nav className="nav">
      <div className="container-fluid flex w-full items-center justify-between rounded-3xl bg-purple-700 text-white p-4 shadow">
        <header>
          <a href="#" className="text-xl font-semibold text-white p-4">
            MediHz
          </a>
        </header>

        {/* For buttons */}
        <div className="flex-container position-relative z-1 inline-flex gap-2 p-0.5 items-center jusify-center">
          {children}
        </div>

        <form className="d-flex border-l border-white/50" role="Export">
          <Button label="Export" />
        </form>
      </div>
    </nav>
  )
}

export interface NavBarItemProp {
  icon?: string
  label: string
  onClick?: (arg0: string) => void
}

export function NavBarItems(prop: NavBarItemProp) {
  return (
    <li>
      {prop.icon && prop.icon}
      <Button label={prop.label} onClick={prop.onClick} />
    </li>
  )
}
