import type { AnnotationRegion } from '../../Interfaces/AnnotationRegion'
import { Badge } from '../Shadcn/badge'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../Shadcn/card'
import { Item, ItemContent, ItemDescription, ItemGroup, ItemTitle } from '../Shadcn/item'

interface IProps {
  annotations: Map<string, AnnotationRegion>
  selectedId: string | null
  onSelect: (id: string) => void
}

export default function AnnotationList({ annotations, selectedId, onSelect }: IProps) {
  const sorted = Array.from(annotations.values()).sort((a, b) => a.startTime - b.startTime)

  return (
    <Card className="max-h-full min-h-0">
      <CardHeader>
        <CardTitle>Annotations</CardTitle>
        <CardDescription>
          {sorted.length === 0
            ? 'Regions you create on the waveform will appear here.'
            : `${sorted.length} ${sorted.length === 1 ? 'region' : 'regions'} in this file`}
        </CardDescription>
      </CardHeader>
      {sorted.length > 0 && (
        <CardContent className="min-h-0 overflow-y-auto">
          <ItemGroup className="gap-1">
            {sorted.map((annotation) => (
              <Item
                key={annotation.id}
                variant={annotation.id === selectedId ? 'muted' : 'default'}
                aria-pressed={annotation.id === selectedId}
                className="text-left hover:bg-muted"
                render={<button type="button" onClick={() => onSelect(annotation.id)} />}
              >
                <ItemContent>
                  <ItemTitle className="tabular-nums">
                    {annotation.startTime.toFixed(2)}s – {annotation.endTime.toFixed(2)}s
                  </ItemTitle>
                  {annotation.authorDisplayName && (
                    <ItemDescription>{annotation.authorDisplayName}</ItemDescription>
                  )}
                  <div className="flex flex-wrap gap-1">
                    {annotation.tags.size === 0 ? (
                      <span className="text-muted-foreground">No tags</span>
                    ) : (
                      Array.from(annotation.tags).map((tag) => (
                        <Badge key={tag} variant="secondary">
                          {tag}
                        </Badge>
                      ))
                    )}
                  </div>
                </ItemContent>
              </Item>
            ))}
          </ItemGroup>
        </CardContent>
      )}
    </Card>
  )
}
