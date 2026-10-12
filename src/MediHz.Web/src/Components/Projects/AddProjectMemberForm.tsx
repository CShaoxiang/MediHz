import { useRef, useState, type FormEvent } from 'react'
import { UserRoundPlus } from 'lucide-react'
import { toast } from 'sonner'
import { addProjectMember } from '../../Api/projects'
import type { AddMemberRole } from '../../Types/projects'
import { Button } from '../Shadcn/button'
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '../Shadcn/dialog'
import { Field, FieldDescription, FieldError, FieldGroup, FieldLabel } from '../Shadcn/field'
import { Input } from '../Shadcn/input'
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../Shadcn/select'
import { Spinner } from '../Shadcn/spinner'

const roles: { value: AddMemberRole; label: string }[] = [
  { value: 'Annotator', label: 'Annotator' },
  { value: 'Viewer', label: 'Viewer' },
]

export default function AddProjectMemberForm({ projectId }: { projectId: string }) {
  const [open, setOpen] = useState(false)
  const [email, setEmail] = useState('')
  const [role, setRole] = useState<AddMemberRole>('Annotator')
  const [pending, setPending] = useState(false)
  const inFlight = useRef(false)
  const [error, setError] = useState('')
  const [added, setAdded] = useState(false)

  function handleOpenChange(nextOpen: boolean) {
    if (inFlight.current) return
    setOpen(nextOpen)
    if (!nextOpen) {
      setEmail('')
      setRole('Annotator')
      setError('')
      setAdded(false)
    }
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (inFlight.current) return
    inFlight.current = true
    setPending(true)
    setError('')
    try {
      const result = await addProjectMember(projectId, email.trim(), role)
      if (result.ok) {
        toast.success(`${result.value.email} now has ${result.value.role} access.`)
        setEmail('')
        setAdded(true)
      } else {
        setError(result.error.detail || result.error.title || 'Unable to add this member.')
      }
    } catch {
      setError('Unable to reach the server. Check your connection and try again.')
    } finally {
      inFlight.current = false
      setPending(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger render={<Button />}>
        <UserRoundPlus data-icon="inline-start" />
        Add member
      </DialogTrigger>
      <DialogContent>
        <form onSubmit={submit} className="contents">
          <DialogHeader>
            <DialogTitle>Add project member</DialogTitle>
            <DialogDescription>
              Grant an existing user access immediately. No email is sent.
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            <Field data-invalid={!!error}>
              <FieldLabel htmlFor="member-email">Email address</FieldLabel>
              <Input
                id="member-email"
                type="email"
                required
                maxLength={320}
                value={email}
                onChange={(event) => {
                  setEmail(event.target.value)
                  setError('')
                }}
                disabled={pending}
                autoComplete="email"
                placeholder="name@example.com"
                aria-invalid={!!error}
              />
              <FieldError>{error}</FieldError>
            </Field>
            <Field>
              <FieldLabel htmlFor="member-role">Project role</FieldLabel>
              <Select
                items={roles}
                value={role}
                onValueChange={(value) => value && setRole(value)}
                disabled={pending}
              >
                <SelectTrigger id="member-role">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    {roles.map((item) => (
                      <SelectItem key={item.value} value={item.value}>
                        {item.label}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
              <FieldDescription>
                {role === 'Annotator'
                  ? 'Can upload audio and create, edit, or delete annotations.'
                  : 'Can play audio, view annotations, and export.'}
              </FieldDescription>
            </Field>
          </FieldGroup>
          <DialogFooter>
            <DialogClose render={<Button variant="outline" />} disabled={pending}>
              {added ? 'Done' : 'Cancel'}
            </DialogClose>
            <Button type="submit" disabled={pending}>
              {pending && <Spinner data-icon="inline-start" />}
              {pending ? 'Adding member' : 'Add member'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
