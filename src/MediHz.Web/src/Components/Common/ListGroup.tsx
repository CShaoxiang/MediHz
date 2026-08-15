interface Props {
  items: string[]
  heading: string
  onSelect: (item: string) => void
}

function ListGroup({ items, heading, onSelect }: Props) {
  return (
    <>
      <h4>{heading}</h4>
      {items.length === 0 && <p>No item found</p>}
      <ul className="list-group mt-2 min-h-0 flex-1 overflow-auto">
        {items.map((item) => (
          <li
            className="rounded-full px-4 py-2 transition hover:bg-white/25"
            key={item}
            onClick={() => onSelect(item)}
          >
            {item}
          </li>
        ))}
      </ul>
    </>
  )
}

export default ListGroup
