import SideBar from './Components/SideBar'
import NavBar from './Components/NavBar'
import Button from './Components/Button'
import Modal from './Components/Modal'
import { useState } from 'react'

function App() {
  const handleSelectItem = (item: string) => {
    console.log(item)
  }

  const [openModal, setModal] = useState(false)

  const changeModal = () => {
    setModal((current) => !current)
  }

  return (
    <main className="App">
      <header className="p-3">
        <NavBar>
          <Button label="Hello" onClick={handleSelectItem} />
          <Button label="Hello" onClick={handleSelectItem} />
          <Button label="Hello" onClick={handleSelectItem} />
          <Button label="Modal" onClick={changeModal} />
          <Modal modalHeader="Modal Header" state={openModal} setState={setModal}>
            <p>Modal body content</p>
          </Modal>
        </NavBar>
      </header>

      <aside>
        <SideBar
          title="Audio Files"
          buttons={[
            { label: 'Button 1', onClick: handleSelectItem },
            { label: 'Button 2', onClick: handleSelectItem },
          ]}
        />
        <SideBar
          title="Annotations"
          lists={[
            {
              heading: 'List 1',
              items: ['Item 1', 'Item 2', 'Item 3', 'Item 4', 'Item 5'],
              onSelect: handleSelectItem,
            },
          ]}
        />
      </aside>
    </main>
  )
}

export default App
