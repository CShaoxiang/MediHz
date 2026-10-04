'use client'

import {
  DropdownMenu,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from './dropdown-menu'
import { Button } from './button'
import { FiMoreVertical } from 'react-icons/fi'
import type { ButtonProps } from '../Common/Button.tsx'

export interface DropdownMenuProp {
  title: string
  actions?: ButtonProps[]
}

export function DropdownMenuBasic({ title, actions }: DropdownMenuProp) {
  return (
    <DropdownMenuTrigger>
      <Button variant="outline">
        <FiMoreVertical />
      </Button>
      <DropdownMenu>
        <DropdownMenuLabel>{title}</DropdownMenuLabel>

        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          {actions &&
            actions.map((action) => (
              <DropdownMenuItem className={action.classname} onAction={() => action.onClick()}>
                {action.label}
              </DropdownMenuItem>
            ))}
        </DropdownMenuGroup>
      </DropdownMenu>
    </DropdownMenuTrigger>
  )
}
