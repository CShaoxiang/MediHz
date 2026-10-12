import type { ReactNode } from 'react'
import { FiMoreVertical } from 'react-icons/fi'
import type { MenuAction } from '../../Types/actions'
import { Button } from '../Shadcn/button'
import { Card, CardAction, CardHeader, CardTitle } from '../Shadcn/card'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '../Shadcn/dropdown-menu'

interface IProps {
  title: string
  icon?: ReactNode
  onClick: () => void
  actions?: MenuAction[]
}

export default function ItemCard({ title, icon, onClick, actions }: IProps) {
  return (
    <Card className="relative justify-center transition-colors hover:bg-accent">
      <CardHeader className="items-center">
        <CardTitle className="flex min-w-0 items-center gap-2 [&_svg]:size-5 [&_svg]:shrink-0">
          {icon}
          <button
            type="button"
            onClick={onClick}
            className="truncate text-left outline-none after:absolute after:inset-0 after:rounded-xl focus-visible:after:ring-3 focus-visible:after:ring-ring/50"
          >
            {title}
          </button>
        </CardTitle>
        {actions && actions.length > 0 && (
          <CardAction className="relative self-center">
            <DropdownMenu>
              <DropdownMenuTrigger
                render={<Button variant="ghost" size="icon-sm" aria-label={`${title} actions`} />}
              >
                <FiMoreVertical />
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-auto">
                <DropdownMenuGroup>
                  <DropdownMenuLabel>{title}</DropdownMenuLabel>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  {actions.map((action) => (
                    <DropdownMenuItem
                      key={action.label}
                      variant={action.variant}
                      onClick={action.onClick}
                    >
                      {action.label}
                    </DropdownMenuItem>
                  ))}
                </DropdownMenuGroup>
              </DropdownMenuContent>
            </DropdownMenu>
          </CardAction>
        )}
      </CardHeader>
    </Card>
  )
}
