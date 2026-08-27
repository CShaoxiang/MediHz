import { Route, Routes, useNavigate } from 'react-router-dom';
import MainPage from './Pages/main';
import ProjectOverviewPage from './Pages/ProjectOverview';
import Projects from './Pages/ProjectsListing';
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
        </NavBar>
      </header>

      <Routes>
        <Route index element={<MainPage />} />
        <Route path="/projects">
          <Route index element={<Projects />} />
          <Route path=":projectId" element={<ProjectOverviewPage />} />
        </Route>
      </Routes>
    </main>
  );
}

export default App;
