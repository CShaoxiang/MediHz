import { useEffect, useState } from 'react'
import SideBar from '../Components/Common/SideBar'
import WaveformRender from '../Components/Waveform/WaveformRender'

export default function MainPage() {
  const [apiMessage, setApiMessage] = useState('')

  const handleSelectItem = (item: string) => {
    console.log(item)
  }

  useEffect(() => {
    fetch('http://localhost:5001/api/sample')
      .then((response) => response.json())
      .then((data) => setApiMessage(data.message))
      .catch((error) => console.error('API error:', error))
  }, [])

  return (
    <div className="flex items-start gap-4">
      <div className="flex items-start gap-4 mr-4">
        <aside className="flex flex-col gap-4">
          <SideBar
            title="Audio Files"
            buttons={[
              { label: 'Audio 1', onClick: handleSelectItem },
              { label: 'Audio 2', onClick: handleSelectItem },
            ]}
          />

          <SideBar
            title="Annotations"
            lists={[
              {
                heading: 'Existing Tags',
                items: ['Inhale', 'Exhale', 'Wheeze', 'Crackle'],
                onSelect: handleSelectItem,
              },
            ]}
          />
        </aside>
      </div>
        <p>{apiMessage}</p>
        <div className="w-full">
          <WaveformRender audio="src\TestAudio\Test.mp3" />
        </div>
      
    </div>
  )
}
