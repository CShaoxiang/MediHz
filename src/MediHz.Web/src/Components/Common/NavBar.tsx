import type { ReactNode } from 'react'
import { Link } from 'react-router-dom'
import Button from './Button'
import { useAuth } from '../../Auth/AuthContext'

interface NavBarProp {
  children?: ReactNode
}

export default function NavBar({ children }: NavBarProp) {
  const { user, logout } = useAuth()

  return (
    <nav className="nav">
      <div className="flex w-full items-center justify-between rounded-3xl bg-purple-700 text-white p-4 shadow">
        <header>
          <a href="#" className="text-xl font-semibold text-white p-4">
            MediHz
          </a>
        </header>

        {/* For buttons */}
        <div className="flex-container relative z-10 inline-flex gap-2 p-0.5 items-center jusify-center">
          {children}
        </div>

        <form className="flex border-l border-white/50" role="Export">
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
        </form>
      </div>
    </nav>
  )
}

export interface NavBarItemProp {
  icon?: string
  label: string
  onClick?: () => void
}

export function NavBarItems(prop: NavBarItemProp) {
  return (
    <li>
      {prop.icon && prop.icon}
      <Button label={prop.label} onClick={prop.onClick} />
    </li>
  )
}
