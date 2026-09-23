import { Route, Routes, useLocation, useNavigate } from 'react-router-dom'
import ProtectedRoute from './Auth/ProtectedRoute'
import Button from './Components/Common/Button'
import NavBar from './Components/Common/NavBar'
import LoginPage from './Pages/login'
import MainPage from './Pages/main'
import ProjectOverviewPage from './Pages/ProjectOverview'
import Projects from './Pages/ProjectsListing'
import LabelsPage from './Pages/LabelsPage'
import AnnotationPage from './Pages/annotationPage'

function App() {
  const navigate = useNavigate()

  const location = useLocation()
  const isLoginPage = location.pathname === '/login'

  return (
    <main className="App p-3">
      {!isLoginPage && (
        <header className="pb-3">
          <NavBar>
            <Button label="Home" onClick={() => navigate('/')} />
            <Button label="Projects" onClick={() => navigate('/projects')} />
            <Button label="Labels" onClick={() => navigate('/labels')} />
          </NavBar>
        </header>
      )}

      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/login" element={<LoginPage />} />
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
        <Route
          path="/projects/:projectId/:audioId"
          element={<ProtectedRoute>
            <AnnotationPage/>
          </ProtectedRoute>
          }
        />
        <Route
          path="/labels"
          element={
            <ProtectedRoute>
              <LabelsPage />
            </ProtectedRoute>
          }
        />
      </Routes>
    </main>
  )
}

export default App
