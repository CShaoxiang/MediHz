import { useRef, useState, type FormEvent } from 'react'
import { Dialog } from '@base-ui/react/dialog'
import { UserRoundPlus, X } from 'lucide-react'
import { addProjectMember } from '../../Api/projects'
import type { AddMemberRole } from '../../Types/projects'
import TextInput from '../Common/TextInput'

export default function AddProjectMemberForm({ projectId }: { projectId: string }) {
  const [open, setOpen] = useState(false)
  const [email, setEmail] = useState('')
  const [role, setRole] = useState<AddMemberRole>('Annotator')
  const [pending, setPending] = useState(false)
  const inFlight = useRef(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  function handleOpenChange(nextOpen: boolean) {
    if (inFlight.current) return
    setOpen(nextOpen)
    if (!nextOpen) {
      setEmail('')
      setRole('Annotator')
      setError('')
      setSuccess('')
    }
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (inFlight.current) return
    inFlight.current = true
    setPending(true)
    setError('')
    setSuccess('')
    try {
      const result = await addProjectMember(projectId, email.trim(), role)
      if (result.ok) {
        setSuccess(`${result.value.email} now has ${result.value.role} access.`)
        setEmail('')
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
    <Dialog.Root open={open} onOpenChange={handleOpenChange}>
      <Dialog.Trigger className="inline-flex min-h-10 items-center justify-center gap-2 rounded-full bg-purple-700 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-purple-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-purple-700">
        <UserRoundPlus className="size-4" aria-hidden="true" />
        Add member
      </Dialog.Trigger>
      <Dialog.Portal>
        <Dialog.Backdrop className="fixed inset-0 z-50 bg-slate-950/45" />
        <Dialog.Viewport className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto p-4 sm:p-6">
          <Dialog.Popup
            initialFocus={() => document.getElementById('member-email')}
            className="relative my-auto w-full max-w-md rounded-2xl border border-slate-200 bg-white p-6 text-left shadow-2xl outline-none sm:p-7"
          >
            <div className="pr-10">
              <Dialog.Title className="text-xl font-semibold tracking-tight text-slate-900">
                Add project member
              </Dialog.Title>
              <Dialog.Description className="mt-2 text-sm leading-6 text-slate-600">
                Grant an existing user access immediately. No email is sent.
              </Dialog.Description>
            </div>
            <Dialog.Close
              type="button"
              aria-label="Close add member dialog"
              disabled={pending}
              className="absolute right-5 top-5 rounded-lg p-1.5 text-slate-500 transition-colors hover:bg-slate-100 hover:text-slate-900 focus-visible:outline-2 focus-visible:outline-purple-700 disabled:opacity-50"
            >
              <X className="size-5" aria-hidden="true" />
            </Dialog.Close>
            <form onSubmit={submit} className="mt-6 space-y-4">
              <div>
                <label
                  htmlFor="member-email"
                  className="mb-1.5 block text-sm font-medium text-slate-800"
                >
                  Email address
                </label>
                <TextInput
                  id="member-email"
                  type="email"
                  required
                  maxLength={320}
                  value={email}
                  onChange={(event) => setEmail(event.target.value)}
                  disabled={pending}
                  autoComplete="email"
                  placeholder="name@example.com"
                  aria-describedby={error ? 'member-error' : undefined}
                />
              </div>
              <div>
                <label
                  htmlFor="member-role"
                  className="mb-1.5 block text-sm font-medium text-slate-800"
                >
                  Project role
                </label>
                <select
                  id="member-role"
                  value={role}
                  onChange={(event) => setRole(event.target.value as AddMemberRole)}
                  disabled={pending}
                  className="w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 focus:border-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-700/30"
                >
                  <option value="Annotator">Annotator</option>
                  <option value="Viewer">Viewer</option>
                </select>
                <p className="mt-2 text-sm leading-5 text-slate-600">
                  {role === 'Annotator'
                    ? 'Can upload audio and create, edit, or delete annotations.'
                    : 'Can play audio, view annotations, and export.'}
                </p>
              </div>
              {error && (
                <p
                  id="member-error"
                  role="alert"
                  className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-800"
                >
                  {error}
                </p>
              )}
              {success && (
                <p
                  role="status"
                  className="rounded-lg bg-green-50 px-3 py-2 text-sm text-green-800"
                >
                  {success}
                </p>
              )}
              <div className="flex flex-wrap items-center justify-end gap-3 border-t border-slate-100 pt-4">
                <Dialog.Close
                  type="button"
                  disabled={pending}
                  className="rounded-full px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100 focus-visible:outline-2 focus-visible:outline-purple-700 disabled:opacity-50"
                >
                  {success ? 'Done' : 'Cancel'}
                </Dialog.Close>
                <button
                  type="submit"
                  disabled={pending}
                  className="rounded-full bg-purple-700 px-5 py-2 text-sm font-semibold text-white transition-colors hover:bg-purple-800 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-purple-700 disabled:opacity-50"
                >
                  {pending ? 'Adding member…' : 'Add member'}
                </button>
              </div>
            </form>
          </Dialog.Popup>
        </Dialog.Viewport>
      </Dialog.Portal>
    </Dialog.Root>
  )
}
