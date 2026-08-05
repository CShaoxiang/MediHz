import Card from "../Components/Card";
import NavBar from "../Components/NavBar";

export default function HomePage(){

    return(<main className="HomePage">
        <header className="p-3">
            <NavBar/>
        </header>
        <div className="">
            <div className="grid grid-cols-[repeat(auto-fit,minmax(18rem,1fr))] gap-4 p-4 max-w-7xl mx-auto">
            <Card title= "Audio" text="Audio text"/>

            <Card title= "Audio 2" text="Audio 2 text"/>

            <Card title= "Audio 3" text="Audio 3 text"/>

            <Card title= "Audio 4" text="Audio 4 text"/>
            </div>
        </div>

    </main>)
}