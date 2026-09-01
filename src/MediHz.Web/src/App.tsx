import { Route, Routes, useNavigate } from 'react-router-dom';
import ProtectedRoute from './Auth/ProtectedRoute';
import Button from './Components/Common/Button';
import NavBar from './Components/Common/NavBar';
import LoginPage from './Pages/login';
import MainPage from './Pages/main';
import ProjectOverviewPage from './Pages/ProjectOverview';
import Projects from './Pages/ProjectsListing';

function App() {
    const navigate = useNavigate();

    return (
        <main className="App p-3">
            <header className="pb-3">
                <NavBar>
                    <Button label="Home" onClick={() => navigate('/')} />
                    <Button label="Projects" onClick={() => navigate('/projects')} />
                </NavBar>
            </header>

            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/" element={<MainPage />} />
                <Route
                    path="/projects"
                    element={
                        <ProtectedRoute>
                            <Projects />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/projects/:projectId"
                    element={
                        <ProtectedRoute>
                            <ProjectOverviewPage />
                        </ProtectedRoute>
                    }
                />
            </Routes>
        </main>
    );
}

export default App;
