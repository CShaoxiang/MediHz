import { useEffect, useState } from 'react'
import Card from '../Components/Common/Card'
import Modal from '../Components/Common/Modal'
import CreateLabelGroupForm from '../Components/Labels/CreateLabelGroupForm'
import LabelGroupView from '../Components/Labels/LabelGroupView'
import { fetchLabelGroups } from '../Api/labels'
import { showApiError } from '../Api/helpers'
import type { LabelGroup } from '../Types/labels'
import Grid from '../Components/Common/Grid'

export default function LabelsPage() {
  const [labelGroups, setLabelGroups] = useState<LabelGroup[]>([])
  const [isGroupOpen, setIsGroupOpen] = useState(false)
  const [selectedGroupId, setSelectedGroupId] = useState<string | null>(null)
  const [isGroupViewOpen, setIsGroupViewOpen] = useState(false)

  useEffect(() => {
    fetchLabelGroups()
      .then((result) => {
        if (result.ok) setLabelGroups(result.value)
        else showApiError(result.error)
      })
      .catch(console.error)
  }, [])

  const handleGroupSubmit = () => {
    setIsGroupOpen(false)
    fetchLabelGroups().then((r) => {
      if (r.ok) setLabelGroups(r.value)
    })
  }

  const selectedGroupName =
    selectedGroupId === null
      ? 'Unassigned'
      : (labelGroups.find((g) => g.id === selectedGroupId)?.name ?? '')

  return (
    <>
      <h2 className="mb-4 text-xl font-bold">Label Groups</h2>
      <Grid>
        <Card title={'Create Label Group'} onClick={() => setIsGroupOpen(true)} />
        <Card
          title={'Unassigned Labels'}
          onClick={() => {
            setSelectedGroupId(null)
            setIsGroupViewOpen(true)
          }}
        />
        {labelGroups.map((group) => (
          <Card
            key={group.id}
            title={group.name}
            onClick={() => {
              setSelectedGroupId(group.id)
              setIsGroupViewOpen(true)
            }}
          />
        ))}
      </Grid>
      <Modal modalHeader={''} state={isGroupOpen} setState={setIsGroupOpen}>
        <CreateLabelGroupForm onSubmit={handleGroupSubmit} />
      </Modal>
      <Modal modalHeader={selectedGroupName} state={isGroupViewOpen} setState={setIsGroupViewOpen}>
        <LabelGroupView
          groupId={selectedGroupId}
          groupName={selectedGroupName}
          allGroups={labelGroups}
          onClose={() => setIsGroupViewOpen(false)}
        />
      </Modal>
    </>
  )
}
