import WaveformRender from '../Components/Waveform/WaveformRender'

export default function MainPage() {
  return (
    <div className="flex items-start gap-4">
      <div className="flex items-start gap-4 mr-4"></div>
      <div className="w-full">
        <WaveformRender audio="src\TestAudio\Test.mp3" />
      </div>
    </div>
  )
}
