import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'

export function highlightColour(
  annotation: AnnotationRegion | null,
  currentUserId: string | undefined,
  spectrogram: boolean,
) {
  let colour: string
  if (!annotation) {
    colour = 'var(--region-highlighting)'
  } else if (!annotation.saved) {
    colour = 'var(--region-unsaved)'
  } else if (annotation.changed){
    colour = 'var(--region-changed)'
  } else if (!annotation.authorId || annotation.authorId === currentUserId) {
    colour = 'var(--region-own)'
  } else {
    // Logic to skip the already used hues above
    const offset = parseInt(annotation.authorId.slice(0, 8), 16) % 240
    const hue = offset < 170 ? 105 + offset : (offset + 165) % 360
    colour = `oklch(0.72 0.16 ${hue})`
  }
  return `color-mix(in oklch, ${colour} ${spectrogram ? 45 : 30}%, transparent)`
}
