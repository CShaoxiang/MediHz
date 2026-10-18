import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../Shadcn/select'
import type { LabelGroup } from '../../Types/labels'

interface IProps {
  id?: string
  'aria-label'?: string
  className?: string
  disabled?: boolean
  groups: LabelGroup[]
  value: string | null
  onValueChange: (groupId: string | null) => void
}

export default function LabelGroupSelect({
  groups,
  value,
  onValueChange,
  className,
  disabled,
  ...triggerProps
}: IProps) {
  const items = [
    { value: null, label: 'Unassigned' },
    ...groups.map((group) => ({ value: group.id, label: group.name })),
  ]

  return (
    <Select items={items} value={value} onValueChange={onValueChange} disabled={disabled}>
      <SelectTrigger className={className} disabled={disabled} {...triggerProps}>
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectGroup>
          {items.map((item) => (
            <SelectItem key={item.value ?? ''} value={item.value}>
              {item.label}
            </SelectItem>
          ))}
        </SelectGroup>
      </SelectContent>
    </Select>
  )
}
