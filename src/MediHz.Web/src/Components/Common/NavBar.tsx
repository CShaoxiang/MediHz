import type { ReactNode } from 'react'
import { Link, useMatch } from 'react-router-dom'
import Button from './Button'
import Dropdown from './Dropdown'
import { useAuth } from '../../Auth/AuthContext'
import { exportProject, exportFile } from '../../Api/exports'
import { downloadJsonFile } from '../../Utilities/download'

interface NavBarProp {
  children?: ReactNode
}

export default function NavBar({ children }: NavBarProp) {
  const { user, logout } = useAuth()
  const projectMatch = useMatch('/projects/:projectId')
  const fileMatch = useMatch('/projects/:projectId/:audioId')
  const projectId = projectMatch?.params.projectId || fileMatch?.params.projectId
  const audioId = fileMatch?.params.audioId

  const handleExportProject = async () => {
    if (!projectId) {
      alert('Please open a project to export it.')
      return
    }

    try {
      const result = await exportProject(projectId)
      if (result.ok) {
        const filename = `${result.value.project_name || 'project'}_export.json`
        downloadJsonFile(result.value, filename)
      } else {
        console.error('Failed to export project:', result.error)
        alert(result.error?.detail || 'Failed to export project.')
      }
    } catch (error) {
      console.error('Error exporting project:', error)
      alert('An unexpected error occurred while exporting the project.')
    }
  }

  const handleExportFile = async () => {
    if (!audioId) {
      alert('Please open a file to export it.')
      return
    }

    try {
      const result = await exportFile(audioId)
      if (result.ok) {
        const filename = `${result.value.file_name || 'file'}_export.json`
        downloadJsonFile(result.value, filename)
      } else {
        console.error('Failed to export file:', result.error)
        alert(result.error?.detail || 'Failed to export file.')
      }
    } catch (error) {
      console.error('Error exporting file:', error)
      alert('An unexpected error occurred while exporting the file.')
    }
  }

  const exportButtons = [
    {
      label: 'Export Project',
      onClick: handleExportProject,
    },
    {
      label: 'Export File',
      onClick: handleExportFile,
    },
  ]

  return (
    <nav className="nav">
      <div className="flex w-full items-center justify-between rounded-3xl bg-purple-700 text-white p-4 shadow-lg">
        <header>
          <a href="#" className="text-xl font-semibold text-white p-4">
            MediHz
          </a>
        </header>

        {/* For buttons */}
        <div className="flex-container relative inline-flex gap-2 p-0.5 items-center jusify-center">
          {children}
        </div>

        <form
          className="flex items-center gap-2 border-l border-white/50 pl-4"
          role="Export"
          onSubmit={(e) => e.preventDefault()}
        >
          <Dropdown label="Export" buttons={exportButtons} />
          {user ? (
            <Button label="Sign out" onClick={() => logout()} />
          ) : (
            <Link
              to="/login"
              className="button roundered-3xl bg-purple-700 px-4 py-2 text-left text-sm font-medium text-white no-underline transition hover:bg-purple-600"
            >
              Sign in
            </Link>
          )}
        </form>
      </div>
    </nav>
  )
}

export interface NavBarItemProp {
  icon?: string
  label: string
  onClick?: () => void
}

export function NavBarItems(prop: NavBarItemProp) {
  return (
    <li>
      {prop.icon && prop.icon}
      <Button label={prop.label} onClick={() => prop.onClick} />
    </li>
  )
}
