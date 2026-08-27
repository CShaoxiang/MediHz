interface CardProps {
  title: string
  onClick: () => void
}

export default function Card({
  title,
  onClick,
}: CardProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="flex w-full flex-col rounded-lg border-2 border-dotted border-medihzprimary p-4 text-left shadow-lg transition-transform duration-300 hover:scale-105 focus:outline-none focus-visible:ring-2 focus-visible:ring-medihzprimary"
    >
      <h5 className="text-lg font-semibold text-gray-800">{title}</h5>
    </button>
  );
}
