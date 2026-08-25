# Component Library

## Current Developed Components
- Button
- Button Group
- Card
- Dropdown
- List Group
- Modal
- Navigation Bar
- Side Bar
- Slider
- Playbar
- Waveform Render

### `Button`
Properties:

    - label: string
    - onClick: () => void

### `ButtonGroup`
Properties:

    - `Button` properties[]

### `Card`
Properties:

    - imageSrc: string (optional)
    - title: string
    - text: string
    - `Button` properties

### `Dropdown`
Properties:

    - label: string
    - `Button` properties (optional)

### `ListGroup`
Properties:

    - items: string[]
    - heading: string
    - onSelect: () => void

### `Modal`
Properties:

    - modalHeader: string
    - state: boolean
    - setState: Dispatch<SetStateAction<boolean>>
    - children: ReactNode (optional)

### `NavBar`
Properties:

    - children: ReactNode (optional)

### `SideBar`
Properties:

    - title: string (optional)
    - `Button` properties[] (optional)
    - `list` properties[] (optional)
    - children?: ReactNode (optional)

### `Slider`
Properties: 

    - min: int
    - max: int
    - state: number (the variable produced from a useState)
    - onChange: (event: any) => void (usually to call the set function of [state])

### `Playbar`
Properties:

    - audioDuration: RefObject<number|null>
    - state: number
    - onChange:  Dispatch<SetStateAction<number>>


### `WaveformRender`
Properties: (Not finalised)

    - audio: string (audio filepath)