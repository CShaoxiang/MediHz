interface AnnotationRegion {
  id: number
  label: string
  startPercent: number
  widthPercent: number
}

const mockAnnotations: AnnotationRegion[] = [
  {
    id: 1,
    label: 'Annotation 1',
    startPercent: 18,
    widthPercent: 18,
  },
  {
    id: 2,
    label: 'Annotation 2',
    startPercent: 34,
    widthPercent: 20,
  },
  {
    id: 3,
    label: 'Annotation 3',
    startPercent: 68,
    widthPercent: 18,
  },
]

export default function WaveformView() {
  return (
    <section className="flex min-w-0 flex-1 flex-col gap-3 px-4">
      <header className="flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold">Audio 1</h2>
          <p className="text-sm text-gray-500">Client-side waveform design preview</p>
        </div>
      </header>

      <div className="relative h-64 overflow-hidden rounded-3xl bg-gray-200 shadow-inner">
        <div className="absolute inset-x-8 top-1/2 h-0.5 bg-gray-500" />

        <div className="absolute left-[18%] top-[42%] h-8 w-[2px] rotate-[-25deg] bg-gray-500" />
        <div className="absolute left-[22%] top-[35%] h-16 w-[2px] rotate-[20deg] bg-gray-500" />
        <div className="absolute left-[27%] top-[30%] h-24 w-[2px] rotate-[-10deg] bg-gray-500" />
        <div className="absolute left-[31%] top-[40%] h-12 w-[2px] rotate-[25deg] bg-gray-500" />

        {mockAnnotations.map((annotation) => (
          <div
            key={annotation.id}
            className="absolute top-0 h-full rounded-2xl border border-purple-800/30 bg-purple-400/35 p-2"
            style={{
              left: `${annotation.startPercent}%`,
              width: `${annotation.widthPercent}%`,
            }}
          >
            <span className="text-xs font-medium text-gray-800">{annotation.label}</span>
          </div>
        ))}
      </div>

      <footer className="flex items-center justify-between rounded-2xl border border-gray-200 px-4 py-3">
        <div className="flex items-center gap-3">
          <button
            type="button"
            className="button rounded-full bg-purple-700 px-4 py-2 text-sm font-medium text-white hover:bg-purple-600"
          >
            Play
          </button>

          <span className="text-sm text-gray-600">00:00 / 00:30</span>
        </div>

        <div className="flex items-center gap-2">
          <span className="text-sm text-gray-600">Zoom</span>

          <button type="button" className="rounded-lg border px-3 py-1">
            −
          </button>

          <button type="button" className="rounded-lg border px-3 py-1">
            +
          </button>
        </div>
      </footer>
    </section>
  )
}
