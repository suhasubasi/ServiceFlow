import { useState, useEffect } from 'react'
import { 
  Wrench, 
  CheckCircle2, 
  Clock, 
  AlertCircle, 
  Trash2, 
  Check, 
  Archive,
  RefreshCw 
} from 'lucide-react'
import type { ServiceTicket } from './types'
import { TicketStatus, TicketPriority } from './types'
import { getTickets, resolveTicket, closeTicket, deleteTicket } from './api'

function App() {
  const [tickets, setTickets] = useState<ServiceTicket[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // 1. Fetch tickets from C# API on page load
  const loadTickets = async () => {
    try {
      setLoading(true)
      setError(null)
      const data = await getTickets()
      setTickets(data)
    } catch (err) {
      setError('Could not connect to API. Is ASP.NET Core running on port 5021?')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadTickets()
  }, [])

  // 2. Action: Resolve a ticket
  const handleResolve = async (id: string) => {
    try {
      await resolveTicket(id)
      await loadTickets() // Refresh list from database
    } catch (err) {
      alert('Failed to resolve ticket.')
    }
  }

  // 3. Action: Close a ticket
  const handleClose = async (id: string) => {
    try {
      await closeTicket(id)
      await loadTickets() // Refresh list from database
    } catch (err) {
      alert('Failed to close ticket.')
    }
  }

  // 4. Action: Delete a ticket
  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this ticket?')) return
    try {
      await deleteTicket(id)
      await loadTickets() // Refresh list from database
    } catch (err) {
      alert('Failed to delete ticket.')
    }
  }

  // Helper for Status Badge styling
  const getStatusBadge = (status: number) => {
    switch (status) {
      case TicketStatus.Open:
        return <span className="inline-flex items-center gap-1 bg-amber-100 text-amber-800 text-xs font-semibold px-2.5 py-1 rounded-full"><Clock className="w-3.5 h-3.5" /> Open</span>
      case TicketStatus.InProgress:
        return <span className="inline-flex items-center gap-1 bg-blue-100 text-blue-800 text-xs font-semibold px-2.5 py-1 rounded-full"><Wrench className="w-3.5 h-3.5" /> In Progress</span>
      case TicketStatus.Resolved:
        return <span className="inline-flex items-center gap-1 bg-emerald-100 text-emerald-800 text-xs font-semibold px-2.5 py-1 rounded-full"><CheckCircle2 className="w-3.5 h-3.5" /> Resolved</span>
      case TicketStatus.Closed:
        return <span className="inline-flex items-center gap-1 bg-slate-100 text-slate-700 text-xs font-semibold px-2.5 py-1 rounded-full"><Archive className="w-3.5 h-3.5" /> Closed</span>
      default:
        return null
    }
  }

  // Helper for Priority Badge styling
  const getPriorityBadge = (priority: number) => {
    switch (priority) {
      case TicketPriority.Critical:
        return <span className="text-xs font-bold text-rose-600 bg-rose-50 border border-rose-200 px-2 py-0.5 rounded">CRITICAL</span>
      case TicketPriority.High:
        return <span className="text-xs font-bold text-orange-600 bg-orange-50 border border-orange-200 px-2 py-0.5 rounded">HIGH</span>
      case TicketPriority.Medium:
        return <span className="text-xs font-semibold text-amber-600 bg-amber-50 border border-amber-200 px-2 py-0.5 rounded">MEDIUM</span>
      case TicketPriority.Low:
        return <span className="text-xs font-semibold text-slate-600 bg-slate-50 border border-slate-200 px-2 py-0.5 rounded">LOW</span>
      default:
        return null
    }
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      {/* Top Navbar */}
      <header className="bg-white border-b border-slate-200 sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-4 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="bg-blue-600 text-white p-2 rounded-lg">
              <Wrench className="w-6 h-6" />
            </div>
            <div>
              <h1 className="text-xl font-bold tracking-tight text-slate-900">ServiceFlow</h1>
              <p className="text-xs text-slate-500">Service Desk & Dispatch Management</p>
            </div>
          </div>
          <button
            onClick={loadTickets}
            className="inline-flex items-center gap-2 text-sm text-slate-600 hover:text-slate-900 bg-slate-100 hover:bg-slate-200 px-3 py-1.5 rounded-md transition"
          >
            <RefreshCw className="w-4 h-4" />
            Refresh
          </button>
        </div>
      </header>

      {/* Main Content Area */}
      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Error Banner */}
        {error && (
          <div className="mb-6 p-4 bg-rose-50 border border-rose-200 rounded-lg text-rose-700 flex items-center gap-3">
            <AlertCircle className="w-5 h-5 shrink-0" />
            <span className="text-sm font-medium">{error}</span>
          </div>
        )}

        {/* Loading Spinner */}
        {loading ? (
          <div className="text-center py-16">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-600 border-t-transparent mb-3"></div>
            <p className="text-sm text-slate-500 font-medium">Connecting to PostgreSQL database...</p>
          </div>
        ) : tickets.length === 0 ? (
          /* Empty State */
          <div className="bg-white rounded-xl border border-slate-200 p-12 text-center max-w-md mx-auto">
            <Wrench className="w-12 h-12 text-slate-300 mx-auto mb-3" />
            <h3 className="text-base font-semibold text-slate-800 mb-1">No Tickets Found</h3>
            <p className="text-sm text-slate-500">
              There are currently no tickets in the database. Use your API or Swagger to add some!
            </p>
          </div>
        ) : (
          /* Tickets Grid */
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {tickets.map((ticket) => (
              <div
                key={ticket.id}
                className="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition p-5 flex flex-col justify-between"
              >
                <div>
                  {/* Status & Priority Row */}
                  <div className="flex items-center justify-between gap-2 mb-3">
                    {getStatusBadge(ticket.status)}
                    {getPriorityBadge(ticket.priority)}
                  </div>

                  {/* Title & Description */}
                  <h3 className="font-semibold text-slate-900 text-lg mb-1 leading-snug">
                    {ticket.title}
                  </h3>
                  <p className="text-slate-600 text-sm mb-4 line-clamp-3">
                    {ticket.description}
                  </p>
                </div>

                {/* Card Footer: Cost, Assignee & Actions */}
                <div className="pt-4 border-t border-slate-100 space-y-3">
                  <div className="flex items-center justify-between text-xs text-slate-500">
                    <span>
                      Est. Cost: <strong className="text-slate-900 text-sm font-semibold">{ticket.estimatedCost?.amount ?? 0} {ticket.estimatedCost?.currency ?? 'SEK'}</strong>
                    </span>
                    <span>
                      {ticket.assignedTo ? `Assigned to: ${ticket.assignedTo}` : 'Unassigned'}
                    </span>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex items-center gap-2 pt-1">
                    {ticket.status !== TicketStatus.Resolved && ticket.status !== TicketStatus.Closed && (
                      <button
                        onClick={() => handleResolve(ticket.id)}
                        className="flex-1 inline-flex items-center justify-center gap-1.5 text-xs font-semibold bg-emerald-600 hover:bg-emerald-700 text-white py-1.5 px-3 rounded-lg transition"
                      >
                        <Check className="w-3.5 h-3.5" />
                        Resolve
                      </button>
                    )}

                    {ticket.status !== TicketStatus.Closed && (
                      <button
                        onClick={() => handleClose(ticket.id)}
                        className="flex-1 inline-flex items-center justify-center gap-1.5 text-xs font-semibold bg-slate-700 hover:bg-slate-800 text-white py-1.5 px-3 rounded-lg transition"
                      >
                        <Archive className="w-3.5 h-3.5" />
                        Close
                      </button>
                    )}

                    <button
                      onClick={() => handleDelete(ticket.id)}
                      className="p-1.5 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
                      title="Delete Ticket"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </main>
    </div>
  )
}

export default App