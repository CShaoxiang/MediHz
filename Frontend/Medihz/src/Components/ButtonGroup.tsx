import type { ButtonProps } from "./Button";

interface ButtonGroupProps {
  buttons?: ButtonProps[];
}

export default function ButtonGroup({ buttons = [] }: ButtonGroupProps) {
  return (
    <div className="button-group rounded-3xl" role="group">
      {buttons.map((button) => (
        <button
          type="button"
          className="button-group-button"
          onClick={() => button.onClick?.(button.label)}
        >
          {button.label}
        </button>
      ))}
    </div>
  );
}
