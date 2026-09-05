import type { ReactNode } from 'react'
import ListGroup from './ListGroup'
import type { ButtonProps } from './Button'
import Button from './Button'

interface ListSection {
  heading: string
  items: string[]
  onSelect?: (item: string) => void
}

interface SideBarProps {
  title?: string
  buttons?: ButtonProps[]
  lists?: ListSection[]
  children?: ReactNode
}

//Side bar can take in both buttons and listgroups
export default function SideBar({ title = 'Sidebar', buttons = [], lists = [] }: SideBarProps) {
  return (
    <aside className="ml-4">
      <nav className="half-sidebar h-full min-h-0 flex flex-col overflow-hidden rounded-3xl bg-purple-700 p-4 text-white shadow">
        <header className="mt-2 mb-4 border-b border-white/20 pb-3 text-center">
          <h2 className="text-xl font-semibold text-white ">{title}</h2>
        </header>
        <div className="flex min-h-0 flex-1 flex-col">
          {buttons.length > 0 && (
            <div className="mb-4 button gap">
              {buttons.map((button) => (
                <Button
                  key={button.label}
                  label={button.label}
                  onClick={() => button.onClick?.()}
                />
              ))}
            </div>
          )}
          {lists.length > 0 ? (
            <div className="list-container min-h-0 flex-1 rounded-2xl bg-white/10 p-3">
              <ListGroup
                items={lists[0]?.items || []}
                heading={lists[0]?.heading || ''}
                onSelect={lists[0]?.onSelect || (() => {})}
              />
            </div>
          ) : null}
        </div>
      </nav>
    </aside>
  )
}
