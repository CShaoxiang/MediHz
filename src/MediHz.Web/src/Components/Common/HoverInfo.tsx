import type { ReactNode } from 'react'

interface HoverInfoProp {
  children: ReactNode
}

export default function HoverInfo({ children }: HoverInfoProp) {
  return <div className="info-box-child">{children}</div>
}
