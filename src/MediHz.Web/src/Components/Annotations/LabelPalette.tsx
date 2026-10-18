import { useMemo, useState, type FormEvent } from 'react'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '../Shadcn/card'
import { Badge } from '../Shadcn/badge'
import { Button } from '../Shadcn/button'
import { Input } from '../Shadcn/input'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectSeparator,
  SelectTrigger,
  SelectValue,
} from '../Shadcn/select'
import { Spinner } from '../Shadcn/spinner'
import { useLabels } from '../../Hooks/useLabels'
import { LuCheck, LuPlus, LuSearch, LuShieldCheck, LuTag, LuTags } from 'react-icons/lu'

export const ALL_GROUPS_ID = '__all__'
export const UNASSIGNED_GROUP_ID = '__unassigned__'

interface LabelPaletteProps {
  activeGroupId: string
  onActiveGroupChange: (groupId: string) => void
  selectedTags?: Set<string>
  onToggleTag: (tagName: string) => void
  canEdit: boolean
  hasSelection: boolean
}

export default function LabelPalette({
  activeGroupId,
  onActiveGroupChange,
  selectedTags,
  onToggleTag,
  canEdit,
  hasSelection,
}: LabelPaletteProps) {
  const { groups, labels, labelsByGroup, prePackagedGroups, customGroups, isLoading, createLabel } =
    useLabels()

  const [searchQuery, setSearchQuery] = useState('')
  const [newLabelName, setNewLabelName] = useState('')
  const [isAdding, setIsAdding] = useState(false)
  const [addError, setAddError] = useState<string | null>(null)

  // Current active group metadata
  const currentGroup = useMemo(
    () => groups.find((g) => g.id === activeGroupId),
    [groups, activeGroupId],
  )
  const isPrePackaged = currentGroup?.ownerId === null

  // Selectable items structure for shadcn/Base UI Select
  const selectItems = useMemo(() => {
    const items: Array<{ value: string; label: string }> = [
      { value: ALL_GROUPS_ID, label: 'All Labels' },
    ]
    prePackagedGroups.forEach((g) => items.push({ value: g.id, label: `${g.name} (Pre-packaged)` }))
    customGroups.forEach((g) => items.push({ value: g.id, label: g.name }))
    items.push({ value: UNASSIGNED_GROUP_ID, label: 'Unassigned Labels' })
    return items
  }, [prePackagedGroups, customGroups])

  // Filter labels based on active group & search text
  const displayedLabels = useMemo(() => {
    const source =
      activeGroupId === ALL_GROUPS_ID
        ? labels
        : activeGroupId === UNASSIGNED_GROUP_ID
          ? (labelsByGroup.get(null) ?? [])
          : (labelsByGroup.get(activeGroupId) ?? [])

    if (!searchQuery.trim()) return source

    const query = searchQuery.trim().toLowerCase()
    return source.filter((l) => l.name.toLowerCase().includes(query))
  }, [activeGroupId, labels, labelsByGroup, searchQuery])

  const handleCreateLabel = async (e: FormEvent) => {
    e.preventDefault()
    const name = newLabelName.trim()
    if (!name) return

    setAddError(null)
    setIsAdding(true)

    // Assign to active group if a custom group is selected, otherwise unassigned
    const targetGroup =
      activeGroupId !== ALL_GROUPS_ID && activeGroupId !== UNASSIGNED_GROUP_ID && !isPrePackaged
        ? activeGroupId
        : null

    const result = await createLabel(name, targetGroup)
    setIsAdding(false)

    if (result.ok) {
      setNewLabelName('')
      if (hasSelection && canEdit) {
        onToggleTag(result.value.name)
      }
    } else {
      setAddError(result.error.detail || 'Failed to create label')
    }
  }

  return (
    <Card className="flex max-h-full min-h-0 flex-col" aria-label="Label Palette">
      <CardHeader className="gap-2 pb-3">
        <div className="flex items-center justify-between gap-2">
          <CardTitle className="flex items-center gap-2 text-base">
            <LuTags className="size-4 text-primary" />
            Label Palette
          </CardTitle>
          {isPrePackaged && (
            <Badge variant="outline" className="flex items-center gap-1 text-xs">
              <LuShieldCheck className="size-3 text-primary" />
              Pre-packaged
            </Badge>
          )}
        </div>
        <CardDescription>
          {hasSelection
            ? canEdit
              ? 'Click a label to tag or untag the selected region.'
              : 'Labels available in this protocol.'
            : 'Select a region on the waveform to apply labels.'}
        </CardDescription>

        <div className="pt-1">
          <Select
            items={selectItems}
            value={activeGroupId}
            onValueChange={(val: unknown) => {
              const id =
                typeof val === 'object' && val !== null && 'value' in val
                  ? String((val as { value: unknown }).value)
                  : typeof val === 'string'
                    ? val
                    : null
              if (id) {
                onActiveGroupChange(id)
              }
            }}
          >
            <SelectTrigger className="w-full" aria-label="Select label group protocol">
              <SelectValue />
            </SelectTrigger>
            <SelectContent alignItemWithTrigger={false}>
              <SelectGroup>
                <SelectItem value={ALL_GROUPS_ID}>All Labels</SelectItem>
              </SelectGroup>
              {prePackagedGroups.length > 0 && (
                <>
                  <SelectSeparator />
                  <SelectGroup>
                    <SelectLabel>Pre-Packaged Protocols</SelectLabel>
                    {prePackagedGroups.map((g) => (
                      <SelectItem key={g.id} value={g.id}>
                        {g.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </>
              )}
              {customGroups.length > 0 && (
                <>
                  <SelectSeparator />
                  <SelectGroup>
                    <SelectLabel>Custom Groups</SelectLabel>
                    {customGroups.map((g) => (
                      <SelectItem key={g.id} value={g.id}>
                        {g.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </>
              )}
              <SelectSeparator />
              <SelectGroup>
                <SelectItem value={UNASSIGNED_GROUP_ID}>Unassigned Labels</SelectItem>
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>

        <div className="relative pt-1">
          <LuSearch className="pointer-events-none absolute top-3.5 left-2.5 size-3.5 text-muted-foreground" />
          <Input
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search labels..."
            className="h-8 pl-8 text-xs"
            aria-label="Filter labels"
          />
        </div>
      </CardHeader>

      <CardContent className="min-h-0 flex-1 overflow-y-auto px-4 py-1">
        {isLoading ? (
          <div className="flex h-32 items-center justify-center text-muted-foreground">
            <Spinner className="mr-2 size-4" />
            Loading labels…
          </div>
        ) : displayedLabels.length === 0 ? (
          <div className="flex h-32 flex-col items-center justify-center gap-1 text-center text-sm text-muted-foreground">
            <LuTag className="size-5 opacity-40" />
            <p>No labels found</p>
            {searchQuery && <p className="text-xs">Try clearing the search query.</p>}
          </div>
        ) : (
          <div className="flex flex-wrap gap-1.5 py-1">
            {displayedLabels.map((lbl, idx) => {
              const isAssigned = selectedTags?.has(lbl.name) ?? false
              return (
                <button
                  key={lbl.id}
                  type="button"
                  disabled={!canEdit || !hasSelection}
                  onClick={() => onToggleTag(lbl.name)}
                  aria-pressed={isAssigned}
                  title={idx < 9 ? `${lbl.name} (Hotkey: ${idx + 1})` : lbl.name}
                  className={`inline-flex items-center gap-1.5 rounded-full border px-2.5 py-1 text-xs font-medium transition-all select-none ${
                    isAssigned
                      ? 'border-primary bg-primary text-primary-foreground shadow-xs hover:bg-primary/90'
                      : 'border-border bg-card text-foreground hover:border-primary/50 hover:bg-muted'
                  } ${
                    !canEdit || !hasSelection
                      ? 'cursor-default opacity-65'
                      : 'cursor-pointer active:scale-95'
                  }`}
                >
                  {isAssigned ? (
                    <LuCheck className="size-3 shrink-0" />
                  ) : (
                    <LuTag className="size-3 shrink-0 opacity-60" />
                  )}
                  <span className="truncate max-w-44">{lbl.name}</span>
                  {idx < 9 && activeGroupId !== ALL_GROUPS_ID && (
                    <span
                      className={`ml-0.5 rounded px-1 py-0.2 text-[10px] tabular-nums font-mono ${
                        isAssigned
                          ? 'bg-primary-foreground/20 text-primary-foreground'
                          : 'bg-muted text-muted-foreground'
                      }`}
                    >
                      {idx + 1}
                    </span>
                  )}
                </button>
              )
            })}
          </div>
        )}
      </CardContent>

      {canEdit && (
        <CardFooter className="flex-col gap-1.5 border-t border-border pt-3">
          <form onSubmit={handleCreateLabel} className="flex w-full gap-2">
            <Input
              value={newLabelName}
              onChange={(e) => {
                setNewLabelName(e.target.value)
                setAddError(null)
              }}
              placeholder={isPrePackaged ? 'Add unassigned label…' : 'Add label to group…'}
              className="h-8 flex-1 text-xs"
              disabled={isAdding}
              aria-label="New label name"
            />
            <Button
              type="submit"
              size="sm"
              disabled={isAdding || !newLabelName.trim()}
              className="h-8 shrink-0 text-xs"
            >
              {isAdding ? <Spinner className="size-3" /> : <LuPlus className="size-3" />}
              Add
            </Button>
          </form>
          {addError && <p className="text-xs text-destructive">{addError}</p>}
        </CardFooter>
      )}
    </Card>
  )
}
