import Card from "../Components/Common/Card";
import AudioUploader from "../Components/AudioUploader"

export default function ProjectOverview() {
    return (
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
    );
}
