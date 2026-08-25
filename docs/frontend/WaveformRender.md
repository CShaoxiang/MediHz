# Waveform Render

## Overview

Using the Wavesurfer API we can load in audio files to produce custom waveforms. Given specific configurations we can also apply different operations and plugins to allow additonal features to interact with the API render.

The API's documentation gives a broad description of its application in frontend but still can be good to look through:
https://wavesurfer.xyz/docs/

This documentation is intended to provide information specifically on how it can be used with the React framework. With the our current intergrated features.

## UseEffect

The waveform render can be initalised during inital loading of the page using a useEffect.

### Example of Intalising

In an useEffect we can create the waveform container and provide an element reference to tell the API which container the waveform should be contained in. Using useRef is recommended to work with the API.

Example of creating a Waveform Render space:

```csharp
const waveformRef = useRef<HTMLDivElement | null>(null) //Would be a HTML elements 'ref'
const waveSurferInstance = useRef<WaveSurfer | null>(null)

useEffect(() => {
if (!waveformRef.current) return

const wavesurfer = WaveSurfer.create({
    container: waveformRef.current,
    waveColor: '#8300db7e',
    progressColor: '#f5076a',
    barWidth: 2,
    barRadius: 10,
    height: 200,
    cursorWidth: 2,
    cursorColor: '#000000',
    normalize: true,
})
}, [])
```

### Example of Web Interactions
With the current version of the waveform rendering, there are some interactions already implemented.

UseEffect hooks also can be applied here to toggle on and off certain features.

Example of toggleable interactions using a useEffect hook:
```csharp
//Manages region selection
  useEffect(() => {
    if (!regionInstance.current) return

    if (toggleHighlight) {
      disableSelection.current = regionInstance.current.enableDragSelection({
        color: 'rgba(181, 50, 68, 0.2)',
      })
      return
    }

    disableSelection.current?.()
    disableSelection.current = null
  }, [toggleHighlight])

```

Some interactions are already built into the API. They can just be called without the need of a useEffect hook (Look through documentation to see which ones)

Example of non useEffect hook toggle:
```csharp
const playPause = () => {
    waveSurferInstance.current?.playPause()
  }

const movePlaybar = (event) => {
    waveSurferInstance.current?.setTime(event.target.value)
    setPlaybar(event.target.value)
  }
```




