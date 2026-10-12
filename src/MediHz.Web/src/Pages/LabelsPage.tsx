import { useEffect, useState } from 'react'
import ItemCard from '../Components/Common/ItemCard'
import { Dialog, DialogContent } from '../Components/Shadcn/dialog'
import CreateLabelGroupForm from '../Components/Labels/CreateLabelGroupForm'
import LabelGroupView from '../Components/Labels/LabelGroupView'
import { fetchLabelGroups } from '../Api/labels'
import { showApiError } from '../Api/helpers'
import type { LabelGroup } from '../Types/labels'
import Grid from '../Components/Common/Grid'
import PageHeader from '../Components/Common/PageHeader'
import EmptyState from '../Components/Common/EmptyState'
import { Button } from '../Components/Shadcn/button'
import { LuPlus, LuTag, LuTags } from 'react-icons/lu'

export default function LabelsPage() {
  const [labelGroups, setLabelGroups] = useState<LabelGroup[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [isGroupOpen, setIsGroupOpen] = useState(false)
  const [selectedGroupId, setSelectedGroupId] = useState<string | null>(null)
  const [isGroupViewOpen, setIsGroupViewOpen] = useState(false)

  const loadLabelGroups = () =>
    fetchLabelGroups()
      .then((result) => {
        if (result.ok) setLabelGroups(result.value)
        else showApiError(result.error)
      })
      .catch(console.error)
      .finally(() => setIsLoading(false))

  useEffect(() => {
    loadLabelGroups()
  }, [])

  const handleGroupSubmit = () => {
    setIsGroupOpen(false)
    loadLabelGroups()
  }

  const handleGroupDeleted = () => {
    setIsGroupViewOpen(false)
    loadLabelGroups()
  }

  const selectedGroupName =
    selectedGroupId === null
      ? 'Unassigned'
      : (labelGroups.find((g) => g.id === selectedGroupId)?.name ?? '')

  return (
    <div className="flex flex-col gap-6">
      <PageHeader
        title="Labels"
        actions={
          <Button onClick={() => setIsGroupOpen(true)}>
            <LuPlus data-icon="inline-start" />
            New label group
          </Button>
        }
      />
      <Grid>
        <ItemCard
          title="Unassigned Labels"
          icon={<LuTags />}
          onClick={() => {
            setSelectedGroupId(null)
            setIsGroupViewOpen(true)
          }}
        />
        {labelGroups.map((group) => (
          <ItemCard
            key={group.id}
            title={group.name}
            icon={<LuTag />}
            onClick={() => {
              setSelectedGroupId(group.id)
              setIsGroupViewOpen(true)
            }}
          />
        ))}
      </Grid>
      {!isLoading && labelGroups.length === 0 && (
        <EmptyState
          icon={<LuTag />}
          title="No label groups yet"
          description="Create a group to organise your labels. Labels without a group appear under Unassigned Labels."
        >
          <Button onClick={() => setIsGroupOpen(true)}>
            <LuPlus data-icon="inline-start" />
            New label group
          </Button>
        </EmptyState>
      )}
      <Dialog open={isGroupOpen} onOpenChange={setIsGroupOpen}>
        <DialogContent>
          <CreateLabelGroupForm onSubmit={handleGroupSubmit} />
        </DialogContent>
      </Dialog>
      <Dialog open={isGroupViewOpen} onOpenChange={setIsGroupViewOpen}>
        <DialogContent>
          <LabelGroupView
            groupId={selectedGroupId}
            groupName={selectedGroupName}
            allGroups={labelGroups}
            onGroupDeleted={handleGroupDeleted}
          />
        </DialogContent>
      </Dialog>
    </div>
  )
}
