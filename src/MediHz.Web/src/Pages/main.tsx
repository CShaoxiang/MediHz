import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import SideBar from "../Components/SideBar";
import NavBar from "../Components/NavBar";
import Button from "../Components/Button";
import Modal from "../Components/Modal";
import WaveformView from "../Components/WaveformView";

export default function MainPage() {
  const [apiMessage, setApiMessage] = useState("");
  const navigate = useNavigate();

  const handleSelectItem = (item: string) => {
    console.log(item);
  };

  const goToProjectOverview = () => {
    navigate("/project-overview");
  };

  useEffect(() => {
    fetch("http://localhost:5001/api/sample")
      .then((response) => response.json())
      .then((data) => setApiMessage(data.message))
      .catch((error) => console.error("API error:", error));
  }, []);

  return (
    <main className="App">
      <header className="p-3">
        <NavBar>
          <Button label="Project Overview" onClick={goToProjectOverview} />
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

        <p>{apiMessage}</p>
        <WaveformView />
      </div>
    </main>
  );
}