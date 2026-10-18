import { useCallback, useEffect, useMemo, useState } from 'react'
import type { Label, LabelGroup } from '../Types/labels'
import { fetchAllLabels, fetchLabelGroups, createLabelInGroup } from '../Api/labels'
import { showApiError } from '../Api/helpers'

export function useLabels() {
  const [groups, setGroups] = useState<LabelGroup[]>([])
  const [labels, setLabels] = useState<Label[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [trigger, setTrigger] = useState(0)

  useEffect(() => {
    let active = true
    async function load() {
      try {
        const [groupsRes, labelsRes] = await Promise.all([fetchLabelGroups(), fetchAllLabels()])
        if (!active) return

        if (groupsRes.ok) {
          setGroups(groupsRes.value)
        } else {
          showApiError(groupsRes.error)
          setError(groupsRes.error.detail || 'Failed to load label groups')
        }

        if (labelsRes.ok) {
          setLabels(labelsRes.value)
        } else {
          showApiError(labelsRes.error)
          setError(labelsRes.error.detail || 'Failed to load labels')
        }
      } catch (err) {
        if (!active) return
        const message = err instanceof Error ? err.message : 'Error loading labels'
        setError(message)
      } finally {
        if (active) setIsLoading(false)
      }
    }
    void load()
    return () => {
      active = false
    }
  }, [trigger])

  const reload = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    setTrigger((prev) => prev + 1)
  }, [])

  const prePackagedGroups = useMemo(() => groups.filter((g) => g.ownerId === null), [groups])

  const customGroups = useMemo(() => groups.filter((g) => g.ownerId !== null), [groups])

  const labelsByGroup = useMemo(() => {
    const map = new Map<string | null, Label[]>()
    // initialize unassigned and all group buckets
    map.set(null, [])
    groups.forEach((g) => map.set(g.id, []))

    labels.forEach((label) => {
      const bucket = map.get(label.labelGroupId)
      if (bucket) {
        bucket.push(label)
      } else {
        map.set(label.labelGroupId, [label])
      }
    })
    return map
  }, [groups, labels])

  const createLabel = useCallback(async (name: string, groupId: string | null = null) => {
    const res = await createLabelInGroup(groupId, name)
    if (res.ok) {
      setLabels((prev) => [...prev, res.value])
    }
    return res
  }, [])

  return {
    groups,
    labels,
    labelsByGroup,
    prePackagedGroups,
    customGroups,
    isLoading,
    error,
    reload,
    createLabel,
  }
}
