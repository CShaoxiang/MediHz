import type { Project } from "../Types/projects";
import { apiGet, apiPostForm } from "./helpers";

const PROJECTS_API_BASE = "Projects"

export async function fetchProjects(): Promise<Project[]> {
  console.info("Fetching all projects");
  return await apiGet<Project[]>(`${PROJECTS_API_BASE}/all`);
}

export async function createProject(project: Project): Promise<Project> {
  console.info("Creating new project:", project);
  return await apiPostForm<Project>(`${PROJECTS_API_BASE}/new`, project);
}
