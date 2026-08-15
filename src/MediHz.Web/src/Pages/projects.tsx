import { useEffect, useState } from "react";
import Card from "../Components/Common/Card";
import Modal from "../Components/Common/Modal";
import CreateProjectForm from "../Components/Projects/CreateProjectForm";
import { fetchProjects } from "../Api/projects";
import type { Project } from "../Types/projects";

export default function ProjectsPage () {
    const [isOpen, setIsOpen] = useState(false);
    const [projects, setProjects] = useState<Project[]>([]);

    useEffect(() => {
      fetchProjects().then((apiProjects) => {
        setProjects(apiProjects)
      }).catch((e) => console.error(e));
    }, [])
    

    const handleOnSubmit = (createdProject: Project) => {
        setProjects(existingProjects => [createdProject, ...existingProjects])
        setIsOpen(false);
    }

    return (
      <>
        <div className="grid grid-cols-5">
          <Card title={"Create Project"} btnProp={{label: "Create", onClick: () => setIsOpen(true)}} />
          {projects.map((project, i) => (
            <Card key={i} title={project.name} btnProp={{label: "Enter", onClick: () => {}}} />
          ))}
        </div>
        <Modal modalHeader={""} state={isOpen} setState={setIsOpen}>
          <CreateProjectForm onSubmit={handleOnSubmit} />
        </Modal>
      </>
    )
}
