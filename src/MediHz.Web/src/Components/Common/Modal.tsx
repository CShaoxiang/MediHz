import type { Dispatch, ReactNode, SetStateAction } from 'react'

interface ModalProp {
  modalHeader: string
  state: boolean
  setState: Dispatch<SetStateAction<boolean>>
  children?: ReactNode
}

export default function Modal({ modalHeader, state, setState, children }: ModalProp) {
  const closeModal = () => {
    setState(false)
  }

  return (
    <>
      {state && (
        <div className="modal" role="dialog" aria-modal="true">
          <div className="overlay" onClick={closeModal}></div>
          <div className="modal-content">
            <h2>{modalHeader}</h2>
            {children}
          </div>
        </div>
      )}
    </>
  )
}
