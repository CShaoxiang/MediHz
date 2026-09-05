interface GridProps {
  children: React.ReactNode
}

export default function Grid({ children }: GridProps) {
  return (
    <div className={`grid gap-4 grid-cols-[repeat(auto-fill,minmax(min(100%,16rem),1fr))]`}>
      {children}
    </div>
  )
}
