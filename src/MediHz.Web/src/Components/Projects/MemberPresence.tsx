import { useProjectPresence } from '../../Hooks/useProjectPresence'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '../Shadcn/tooltip'

export default function MemberPresence({ projectId }: { projectId: string }) {
  return <ProjectMemberPresence key={projectId} projectId={projectId} />
}

function ProjectMemberPresence({ projectId }: { projectId: string }) {
  const { members, onlineIds, membersError, loaded } = useProjectPresence(projectId)
  const sorted = [...members].sort((a, b) => a.userId.localeCompare(b.userId))
  const activeCount = members.filter((member) => onlineIds?.has(member.userId)).length

  return (
    <div
      role="group"
      aria-label="Project member presence"
      className="flex min-w-0 items-center gap-3"
    >
      <TooltipProvider>
        <ul
          aria-label="Project members"
          className="flex max-w-64 flex-wrap items-center gap-y-2 -space-x-2"
        >
          {sorted.map((member, index) => {
            const online = onlineIds?.has(member.userId) ?? false
            const status = onlineIds === null ? 'Status unavailable' : online ? 'Online' : 'Offline'
            const name = member.displayName || member.email
            const initials = name
              .trim()
              .split(/\s+/)
              .slice(0, 2)
              .map((part) => part[0])
              .join('')
              .toUpperCase()
            return (
              <li key={member.userId} className="relative hover:z-10 focus-within:z-10">
                <Tooltip>
                  <TooltipTrigger
                    type="button"
                    aria-label={`${name} — ${status}`}
                    className="relative flex size-9 items-center justify-center rounded-full border-2 border-background text-xs font-semibold text-white shadow-sm outline-none transition-transform hover:-translate-y-0.5 focus-visible:z-10 focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                    style={{
                      backgroundColor: online
                        ? `hsl(${(index * 137.508 + 265) % 360} 65% 35%)`
                        : '#6b7280',
                    }}
                  >
                    <span aria-hidden="true">{initials}</span>
                    <span
                      aria-hidden="true"
                      className={`absolute -right-0.5 -bottom-0.5 size-3 rounded-full border-2 border-background ${online ? 'bg-emerald-500' : 'bg-gray-400'}`}
                    />
                  </TooltipTrigger>
                  <TooltipContent side="bottom">
                    <span className="flex flex-col gap-0.5">
                      <span className="font-semibold">{name}</span>
                      <span>
                        {member.role} · {status}
                        {online ? ' in this project' : ''}
                      </span>
                    </span>
                  </TooltipContent>
                </Tooltip>
              </li>
            )
          })}
        </ul>
      </TooltipProvider>
      <div className="flex shrink-0 flex-col text-xs leading-tight" role="status">
        <span className="font-medium">
          {membersError
            ? 'Members unavailable'
            : !loaded
              ? 'Loading members…'
              : onlineIds === null
                ? 'Presence unavailable'
                : `${activeCount} online`}
        </span>
        <span className="text-muted-foreground">
          {loaded ? `${members.length} members` : 'Project members'}
        </span>
      </div>
    </div>
  )
}
