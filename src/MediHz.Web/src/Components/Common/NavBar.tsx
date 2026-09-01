import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import Button from './Button'
import { useAuth } from '../../Auth/AuthProvider'

interface NavBarProp {
  children?: ReactNode
}

export default function NavBar({ children }: NavBarProp) {
  const { user, logout } = useAuth()

  return (
    <nav className="nav">
      <div className="container-fluid flex w-full items-center justify-between rounded-3xl bg-purple-700 text-white p-4 shadow">
        <header>
          <a href="#" className="text-xl font-semibold text-white p-4">
            MediHz
          </a>
        </header>

        <div className="flex-container position-relative z-1 inline-flex gap-2 p-0.5 items-center jusify-center">
          {children}
        </div>

        <div className="flex items-center gap-2 border-l border-white/50 pl-4">
          <Button label="Export" />
          {user ? (
            <Button label="Sign out" onClick={() => logout()} />
          ) : (
            <Link
              to="/login"
              className="button roundered-3xl bg-purple-700 px-4 py-2 text-left text-sm font-medium text-white no-underline transition hover:bg-purple-600"
            >
              Sign in
            </Link>
          )}
        </div>
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
