import { useEffect, useState } from 'react'
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr'
import { getToken } from '../Auth/tokenStorage'
import { fetchProjectMembers } from '../Api/projects'
import type { ProjectMember } from '../Types/projects'

type PresenceSnapshot = { projectId: string; version: number; onlineUserIds: string[] }

export function useProjectPresence(projectId: string) {
  const [members, setMembers] = useState<ProjectMember[]>([])
  const [onlineIds, setOnlineIds] = useState<Set<string> | null>(null)
  const [membersError, setMembersError] = useState(false)
  const [loaded, setLoaded] = useState(false)

  useEffect(() => {
    let disposed = false
    let version = 0
    let retry: ReturnType<typeof setTimeout> | undefined
    let loadingMembers = false
    const hub = new HubConnectionBuilder()
      .withUrl(`/hubs/presence?projectId=${encodeURIComponent(projectId)}`, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => getToken() || '',
      })
      .withAutomaticReconnect({ nextRetryDelayInMilliseconds: () => 5000 })
      .build()

    async function loadMembers() {
      if (loadingMembers) return
      loadingMembers = true
      try {
        const result = await fetchProjectMembers(projectId)
        if (disposed) return
        setMembersError(!result.ok)
        if (result.ok) {
          setMembers(result.value)
          setLoaded(true)
        }
      } catch {
        if (!disposed) setMembersError(true)
      } finally {
        loadingMembers = false
      }
    }

    hub.on('PresenceChanged', (snapshot: PresenceSnapshot) => {
      if (disposed || snapshot.projectId !== projectId || snapshot.version < version) return
      version = snapshot.version
      setOnlineIds(new Set(snapshot.onlineUserIds))
    })
    const unavailable = () => {
      version = 0
      if (!disposed) setOnlineIds(null)
    }
    hub.onreconnecting(unavailable)
    hub.onreconnected(() => {
      void loadMembers()
    })
    hub.onclose(unavailable)

    async function connect() {
      try {
        await hub.start()
        if (disposed) await hub.stop()
      } catch {
        if (!disposed) {
          unavailable()
          retry = setTimeout(() => {
            void connect()
          }, 5000)
        }
      }
    }

    void loadMembers()
    void connect()
    const refresh = setInterval(() => {
      void loadMembers()
    }, 30_000)
    const memberAdded = () => {
      void loadMembers()
    }
    window.addEventListener('project-members-changed', memberAdded)
    return () => {
      disposed = true
      clearTimeout(retry)
      clearInterval(refresh)
      window.removeEventListener('project-members-changed', memberAdded)
      void hub.stop().catch(() => undefined)
    }
  }, [projectId])

  return { members, onlineIds, membersError, loaded }
}
