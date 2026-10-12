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
  groups: LabelGroup[]
  value: string | null
  onValueChange: (groupId: string | null) => void
}

export default function LabelGroupSelect({
  groups,
  value,
  onValueChange,
  className,
  ...triggerProps
}: IProps) {
  const items = [
    { value: null, label: 'Unassigned' },
    ...groups.map((group) => ({ value: group.id, label: group.name })),
  ]

  return (
    <Select items={items} value={value} onValueChange={onValueChange}>
      <SelectTrigger className={className} {...triggerProps}>
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
