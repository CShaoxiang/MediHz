import Card from "../Components/Card";
import NavBar from "../Components/NavBar";
import AudioUploader from "../Components/AudioUploader"
import { Link } from "react-router-dom";

export default function ProjectOverview() {
    return (
        <main className="ProjectOverview">
            <header className="p-3">
                <NavBar>
                    <Link to="/" className="text-white hover:text-gray-200 px-4 py-2 bg-purple-600 rounded-3xl transition-colors">
                        Back to Main
                    </Link>
                </NavBar>
            </header>
            <div className="">
                <h1 className="text-2xl font-bold p-4 text-center">Project Overview</h1>
                <div className="grid grid-cols-[repeat(auto-fit,minmax(18rem,1fr))] gap-4 p-4 max-w-7xl mx-auto">
                    <AudioUploader/>
                    <Card title="Audio 1" text="Placeholder for audio file 1"/>
                    <Card title="Audio 2" text="Placeholder for audio file 2"/>
                    <Card title="Audio 3" text="Placeholder for audio file 3"/>
                    <Card title="Audio 4" text="Placeholder for audio file 4"/>
                </div>
            </div>
        </main>
    );
}
