import { useState, type ReactNode } from "react";
import Button from "./Button";

interface ModalProp {
  buttonLabel: string;
  modalHeader: string;
  children?: ReactNode;
}

export default function Modal({
  buttonLabel,
  modalHeader,
  children,
}: ModalProp) {
  const [modal, setModal] = useState(false);

  const toggleModal = () => {
    setModal((current) => !current);
  };

  return (
    <>
      <Button label={buttonLabel} onClick={toggleModal} />

      {modal && (
        <div className="modal" role="dialog" aria-modal="true">
          <div className="overlay" onClick={toggleModal}></div>
          <div className="modal-content">
            <h2>{modalHeader}</h2>
            {children}
            <Button label="Close" onClick={toggleModal} />
          </div>
        </div>
      )}
    </>
  );
}
