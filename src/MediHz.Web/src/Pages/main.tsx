import SideBar from "../Components/SideBar";
import NavBar from "../Components/NavBar";
import Button from "../Components/Button";
import Modal from "../Components/Modal";
import WaveformView from "../Components/WaveformView";

export default function MainPage() {
  const handleSelectItem = (item: string) => {
    console.log(item);
  };

  return (
    <main className="App">
      <header className="p-3">
        <NavBar>
          <Button label="Hello" onClick={handleSelectItem} />
          <Button label="Hello" onClick={handleSelectItem} />
          <Button label="Hello" onClick={handleSelectItem} />
          <Modal buttonLabel="Modal" modalHeader="Modal Header">
            <p>Modal body content</p>
          </Modal>
        </NavBar>
      </header>

      <div className="flex items-start gap-4">
        <aside className="flex flex-col gap-4">
          <SideBar
            title="Audio Files"
            buttons={[
              { label: "Audio 1", onClick: handleSelectItem },
              { label: "Audio 2", onClick: handleSelectItem },
            ]}
          />

          <SideBar
            title="Annotations"
            lists={[
              {
                heading: "Existing Tags",
                items: ["Inhale", "Exhale", "Wheeze", "Crackle"],
                onSelect: handleSelectItem,
              },
            ]}
          />
        </aside>

        <WaveformView />
      </div>
    </main>
  );
}