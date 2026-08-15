import { Route, Routes, useNavigate } from 'react-router-dom';
import MainPage from './Pages/main';
import ProjectOverview from './Pages/ProjectOverview';
import Projects from './Pages/projects';
import Button from './Components/Common/Button';
import NavBar from './Components/Common/NavBar';

function App() {
  const navigate = useNavigate();  

  return (
    <main className="App p-3">
      <header className="pb-3">
        <NavBar>
          <Button label="Home" onClick={() => navigate("/")} />
          <Button label="Projects" onClick={() => navigate("/projects")} />
          <Button label="Project Overview" onClick={() => navigate("/project-overview")} />
        </NavBar>
      </header>

      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/projects" element={<Projects />} />
        <Route path="/project-overview" element={<ProjectOverview />} />
      </Routes>
    </main>
  );
}

export default App;
