import { BrowserRouter, NavLink, Route, Routes } from 'react-router-dom';
import MainPage from './Pages/main';
import ProjectOverview from './Pages/ProjectOverview';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/project-overview" element={<ProjectOverview />} />
      </Routes>
    </BrowserRouter>
  );
}

function DetailsPage() {
  return (
    <main className="p-4">
      <h1>Details Page</h1>
      <p>This is a second page in your React app.</p>
    </main>
  );
}

export default App;
