import { Link, Navigate, Route, Routes, useLocation } from 'react-router-dom'
import ProtectedRoute from './Auth/ProtectedRoute'
import { Button } from './Components/Shadcn/button'
import NavBar from './Components/Common/NavBar'
import { Toaster } from './Components/Shadcn/sonner'
import LoginPage from './Pages/login'
import ProjectOverviewPage from './Pages/ProjectOverview'
import Projects from './Pages/ProjectsListing'
import LabelsPage from './Pages/LabelsPage'
import AnnotationPage from './Pages/annotationPage'

function App() {
  const location = useLocation()
  const isLoginPage = location.pathname === '/login'

  return (
    <div className="flex h-screen flex-col gap-3 p-3">
      {!isLoginPage && (
        <header>
          <NavBar>
            <Button variant="secondary" nativeButton={false} render={<Link to="/projects" />}>
              Projects
            </Button>
            <Button variant="secondary" nativeButton={false} render={<Link to="/labels" />}>
              Labels
            </Button>
          </NavBar>
        </header>
      )}

      <main className="min-h-0 flex-1 overflow-y-auto p-2">
        <Routes>
          <Route path="/" element={<Navigate to="/projects" replace />} />
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
            element={
              <ProtectedRoute>
                <AnnotationPage />
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
      <Toaster />
    </div>
  )
}

export default App
