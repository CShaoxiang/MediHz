import type { ReactNode } from 'react'
import { Link, useMatch } from 'react-router-dom'
import { toast } from 'sonner'
import { Button } from '../Shadcn/button'
import { Separator } from '../Shadcn/separator'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '../Shadcn/dropdown-menu'
import { useAuth } from '../../Auth/AuthContext'
import { exportProject, exportFile } from '../../Api/exports'
import { downloadJsonFile } from '../../Utilities/download'

interface NavBarProp {
  children?: ReactNode
}

export default function NavBar({ children }: NavBarProp) {
  const { logout } = useAuth()
  const projectMatch = useMatch('/projects/:projectId')
  const fileMatch = useMatch('/projects/:projectId/:audioId')
  const projectId = projectMatch?.params.projectId || fileMatch?.params.projectId
  const audioId = fileMatch?.params.audioId

  const handleExportProject = async () => {
    if (!projectId) {
      toast.info('Please open a project to export it.')
      return
    }

    try {
      const result = await exportProject(projectId)
      if (result.ok) {
        const filename = `${result.value.project_name || 'project'}_export.json`
        downloadJsonFile(result.value, filename)
      } else {
        console.error('Failed to export project:', result.error)
        toast.error(result.error?.detail || 'Failed to export project.')
      }
    } catch (error) {
      console.error('Error exporting project:', error)
      toast.error('An unexpected error occurred while exporting the project.')
    }
  }

  const handleExportFile = async () => {
    if (!audioId) {
      toast.info('Please open a file to export it.')
      return
    }

    try {
      const result = await exportFile(audioId)
      if (result.ok) {
        const filename = `${result.value.file_name || 'file'}_export.json`
        downloadJsonFile(result.value, filename)
      } else {
        console.error('Failed to export file:', result.error)
        toast.error(result.error?.detail || 'Failed to export file.')
      }
    } catch (error) {
      console.error('Error exporting file:', error)
      toast.error('An unexpected error occurred while exporting the file.')
    }
  }

  return (
    <nav className="flex w-full items-center justify-between rounded-3xl bg-primary p-4 text-primary-foreground shadow-lg">
      <Link to="/projects" className="p-4 text-xl font-semibold">
        MediHz
      </Link>

      <div className="flex items-center gap-2">{children}</div>

      <div className="flex items-center gap-2">
        <Separator orientation="vertical" className="mr-2 bg-primary-foreground/50" />
        <DropdownMenu>
          <DropdownMenuTrigger render={<Button variant="secondary" />}>Export</DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={handleExportProject}>Export Project</DropdownMenuItem>
            <DropdownMenuItem onClick={handleExportFile}>Export File</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
        <Button variant="secondary" onClick={() => logout()}>
          Sign out
        </Button>
      </div>
    </nav>
  )
}
