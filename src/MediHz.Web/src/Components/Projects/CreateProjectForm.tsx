import { useState } from "react";
import Button from "../Common/Button";
import type { Project } from "../../Types/projects";
import { createProject } from "../../Api/projects";

interface IProps {
  onSubmit: (project: Project) => void;
}

export default function CreateProjectForm({onSubmit} : IProps) {
    const [project, setProject] = useState<Project>({
        name: ""
    })

    const validateProject = (project : Project): boolean => {
        return project.name.trim() !== ""
    }
    
    const handleSubmit = () => {
      if (!validateProject(project)) {
        alert("Invalid project details");
        return;
      }

      createProject(project)
      onSubmit(project);
    }

    return (
      <div className="flex flex-col gap-2">
        <h2>Create a New Project</h2>
        <input type="text" placeholder="Name" value={project.name} onChange={(e) => setProject({...project, name: e.target.value})} />
        <Button label="Create +" onClick={handleSubmit} />
      </div>
    )
}
