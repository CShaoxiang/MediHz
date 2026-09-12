import type { ReactNode } from 'react'
import HoverInfo from './HoverInfo'

interface Props {
  items?: string[]
  heading: string
  onSelect?: (item: string) => void
  hoverable?: ReactNode
}

export default function ListGroup({ items = [], heading, onSelect, hoverable }: Props) {
  return (
    <div className="flex h-full min-h-0 flex-col">
      <h4>{heading}</h4>
      {items.length === 0 && <p>No item found</p>}
      <ul className="list-group mt-2 min-h-0 flex-1 overflow-y-auto">
        {items.map((item) => (
          <li
            className="flex items-center rounded-full px-4 py-2 transition hover:bg-white/25 info-box-parent"
            key={item}
            onClick={() => onSelect && onSelect(item)}
          >
            {hoverable && <HoverInfo>{hoverable}</HoverInfo>}
            {item}
          </li>
        ))}
      </ul>
    </div>
  )
}
