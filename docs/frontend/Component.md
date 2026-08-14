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
- Waveform View (Placeholder)

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

### `Navigation Bar`
Properties:

    - children: ReactNode (optional)

### `SideBar`
Properties:

    - title: string (optional)
    - `Button` properties[] (optional)
    - `list` properties[] (optional)
    - children?: ReactNode (optional)

### `WaveformView`
Properties: (Not finalised)

    - id: number
    - label: string
    - startPercent: number
    - widthPercent: number