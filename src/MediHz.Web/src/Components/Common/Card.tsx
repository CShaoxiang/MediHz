import Button, { type ButtonProps } from './Button'

interface CardProp {
  imageSrc?: string
  title: string
  text?: string
  btnProp?: ButtonProps
}

export default function Card({
  imageSrc = 'src/Assets/istockphoto-2148659111-612x612.jpg',
  title = 'Card',
  text,
  btnProp,
}: CardProp) {
  return (
    <div className="card">
      <div className="card-img-top overflow-hidden">
        <img src={imageSrc} alt="..." />
      </div>
      <div className="card-body">
        <h5 className="card-title">{title}</h5>
        <div></div>
        {text && <p className="card-text"> {text} </p>}
        {btnProp && <Button label={btnProp?.label} onClick={btnProp?.onClick} />}
      </div>
    </div>
  )
}
