import { useState, useEffect } from 'react'
import { 
  Wrench, 
  CheckCircle2, 
  Clock, 
  AlertCircle, 
  Trash2, 
  Check, 
  Archive,
  RefreshCw,
  Plus,
  X
} from 'lucide-react'
import type { ServiceTicket, Customer } from './types'
import { TicketStatus, TicketPriority } from './types'
import { 
  getTickets, 
  getCustomers, 
  createTicket, 
  resolveTicket, 
  closeTicket, 
  deleteTicket 
} from './api'

function App() {
  // 1. Dashboard State
  const [tickets, setTickets] = useState<ServiceTicket[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [statusFilter, setStatusFilter] = useState<'all' | TicketStatus>('all')

  // Filtered tickets based on active status pill
  const filteredTickets = statusFilter === 'all'
    ? tickets
    : tickets.filter((ticket) => ticket.status === statusFilter)

  // Count badges for each filter pill
  const counts = {
    all: tickets.length,
    open: tickets.filter((t) => t.status === TicketStatus.Open).length,
    inProgress: tickets.filter((t) => t.status === TicketStatus.InProgress).length,
    resolved: tickets.filter((t) => t.status === TicketStatus.Resolved).length,
    closed: tickets.filter((t) => t.status === TicketStatus.Closed).length,
  }
  // 2. Modal & Form State
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [customers, setCustomers] = useState<Customer[]>([])
  const [submitting, setSubmitting] = useState(false)

  // Form Fields
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [customerId, setCustomerId] = useState('')
  const [priority, setPriority] = useState<TicketPriority>(TicketPriority.Medium)
  const [estimatedCost, setEstimatedCost] = useState<number>(500)

  // 3. Fetch tickets from C# API on page load
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

  // 4. Open Modal & Load Customers for Dropdown
  const openCreateModal = async () => {
    setIsModalOpen(true)
    try {
      const customerData = await getCustomers()
      setCustomers(customerData)
      // Pre-select the first customer if available
      if (customerData.length > 0 && !customerId) {
        setCustomerId(customerData[0].id)
      }
    } catch (err) {
      console.error('Could not load customers for dropdown', err)
    }
  }

  // 5. Close Modal & Reset Form
  const closeCreateModal = () => {
    setIsModalOpen(false)
    setTitle('')
    setDescription('')
    setCustomerId('')
    setPriority(TicketPriority.Medium)
    setEstimatedCost(500)
  }

  useEffect(() => {
    loadTickets()
  }, [])

  // 6. Action: Create Ticket (Form Submit)
  const handleCreateTicket = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!title.trim()) {
      alert('Please enter a ticket title.')
      return
    }

    if (!customerId) {
      alert('Please select a customer.')
      return
    }

    try {
      setSubmitting(true)
      await createTicket({
        title: title.trim(),
        description: description.trim(),
        customerId: customerId,
        priority: Number(priority) as TicketPriority,
        estimatedCostAmount: Number(estimatedCost) || 0
      })

      closeCreateModal()
      await loadTickets() // Refresh ticket grid from database
    } catch (err) {
      alert('Failed to create ticket. Make sure the backend API is running.')
    } finally {
      setSubmitting(false)
    }
  }

  // 7. Action: Resolve a ticket
  const handleResolve = async (id: string) => {
    try {
      await resolveTicket(id)
      await loadTickets() // Refresh list from database
    } catch (err) {
      alert('Failed to resolve ticket.')
    }
  }

  // 8. Action: Close a ticket
  const handleClose = async (id: string) => {
    try {
      await closeTicket(id)
      await loadTickets() // Refresh list from database
    } catch (err) {
      alert('Failed to close ticket.')
    }
  }

  // 9. Action: Delete a ticket
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
            <div className="bg-blue-600 text-white p-2 rounded-lg shadow-sm">
              <Wrench className="w-6 h-6" />
            </div>
            <div>
              <h1 className="text-xl font-bold tracking-tight text-slate-900">ServiceFlow</h1>
              <p className="text-xs text-slate-500">Service Desk & Dispatch Management</p>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <button
              onClick={openCreateModal}
              className="inline-flex items-center gap-1.5 text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 px-3.5 py-1.5 rounded-lg shadow-sm transition"
            >
              <Plus className="w-4 h-4" />
              New Ticket
            </button>
            <button
              onClick={loadTickets}
              className="inline-flex items-center gap-2 text-sm text-slate-600 hover:text-slate-900 bg-slate-100 hover:bg-slate-200 px-3 py-1.5 rounded-lg transition"
            >
              <RefreshCw className="w-4 h-4" />
              Refresh
            </button>
          </div>
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
            <p className="text-sm text-slate-500 mb-4">
              There are currently no tickets in the database. Click below to dispatch your first ticket!
            </p>
            <button
              onClick={openCreateModal}
              className="inline-flex items-center gap-1.5 text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 px-4 py-2 rounded-lg shadow-sm transition"
            >
              <Plus className="w-4 h-4" />
              Create First Ticket
            </button>
          </div>
        ) : (
          <>
            {/* Status Filter Pills */}
            <div className="flex flex-wrap items-center gap-2 mb-6">
              <button
                onClick={() => setStatusFilter('all')}
                className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
                  statusFilter === 'all'
                    ? 'bg-blue-600 text-white shadow-sm'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                }`}
              >
                All
                <span className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                  statusFilter === 'all' ? 'bg-blue-700 text-white' : 'bg-slate-100 text-slate-600'
                }`}>
                  {counts.all}
                </span>
              </button>

              <button
                onClick={() => setStatusFilter(TicketStatus.Open)}
                className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
                  statusFilter === TicketStatus.Open
                    ? 'bg-amber-500 text-white shadow-sm'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                }`}
              >
                <Clock className="w-3.5 h-3.5" />
                Open
                <span className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                  statusFilter === TicketStatus.Open ? 'bg-amber-600 text-white' : 'bg-slate-100 text-slate-600'
                }`}>
                  {counts.open}
                </span>
              </button>

              <button
                onClick={() => setStatusFilter(TicketStatus.InProgress)}
                className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
                  statusFilter === TicketStatus.InProgress
                    ? 'bg-blue-600 text-white shadow-sm'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                }`}
              >
                <Wrench className="w-3.5 h-3.5" />
                In Progress
                <span className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                  statusFilter === TicketStatus.InProgress ? 'bg-blue-700 text-white' : 'bg-slate-100 text-slate-600'
                }`}>
                  {counts.inProgress}
                </span>
              </button>

              <button
                onClick={() => setStatusFilter(TicketStatus.Resolved)}
                className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
                  statusFilter === TicketStatus.Resolved
                    ? 'bg-emerald-600 text-white shadow-sm'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                }`}
              >
                <CheckCircle2 className="w-3.5 h-3.5" />
                Resolved
                <span className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                  statusFilter === TicketStatus.Resolved ? 'bg-emerald-700 text-white' : 'bg-slate-100 text-slate-600'
                }`}>
                  {counts.resolved}
                </span>
              </button>

              <button
                onClick={() => setStatusFilter(TicketStatus.Closed)}
                className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
                  statusFilter === TicketStatus.Closed
                    ? 'bg-slate-700 text-white shadow-sm'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
                }`}
              >
                <Archive className="w-3.5 h-3.5" />
                Closed
                <span className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                  statusFilter === TicketStatus.Closed ? 'bg-slate-800 text-white' : 'bg-slate-100 text-slate-600'
                }`}>
                  {counts.closed}
                </span>
              </button>
            </div>

            {/* Tickets Grid or Filter Empty State */}
            {filteredTickets.length === 0 ? (
              <div className="bg-white rounded-xl border border-slate-200 p-8 text-center max-w-sm mx-auto">
                <p className="text-sm text-slate-500 font-medium mb-3">No tickets match this filter.</p>
                <button
                  onClick={() => setStatusFilter('all')}
                  className="text-xs font-semibold text-blue-600 hover:text-blue-800 underline"
                >
                  Show all tickets
                </button>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredTickets.map((ticket) => (
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
                          {ticket.assignedTo ? `Assigned: ${ticket.assignedTo}` : 'Unassigned'}
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
          </>
        )}
      </main>

      {/* CREATE TICKET MODAL DIALOG */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
          <div className="bg-white rounded-2xl shadow-xl border border-slate-200 w-full max-w-lg overflow-hidden">
            {/* Modal Header */}
            <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
              <div>
                <h2 className="text-lg font-bold text-slate-900">New Ticket</h2>
                <p className="text-xs text-slate-500">Register a new customer issue or repair request.</p>
              </div>
              <button
                onClick={closeCreateModal}
                className="text-slate-400 hover:text-slate-600 p-1 rounded-lg hover:bg-slate-100 transition"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Modal Form */}
            <form onSubmit={handleCreateTicket} className="p-6 space-y-4">
              {/* Title Field */}
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">
                  Title *
                </label>
                <input
                  type="text"
                  required
                  placeholder="e.g. Office Wi-Fi keeps disconnecting"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>

              {/* Description Field */}
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">
                  Description
                </label>
                <textarea
                  rows={3}
                  placeholder="Add any details or notes about the problem..."
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
                />
              </div>

              {/* Customer Selector */}
              <div>
                <label className="block text-sm font-medium text-slate-700 mb-1">
                  Customer
                </label>
                {customers.length > 0 ? (
                  <select
                    value={customerId}
                    onChange={(e) => setCustomerId(e.target.value)}
                    className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  >
                    {customers.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.fullName} ({c.companyName || 'Private'})
                      </option>
                    ))}
                  </select>
                ) : (
                  <input
                    type="text"
                    placeholder="Enter customer name or ID..."
                    value={customerId}
                    onChange={(e) => setCustomerId(e.target.value)}
                    className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                )}
              </div>

              {/* Priority & Cost Row */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">
                    Priority
                  </label>
                  <select
                    value={priority}
                    onChange={(e) => setPriority(Number(e.target.value) as TicketPriority)}
                    className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
                  >
                    <option value={TicketPriority.Low}>Low</option>
                    <option value={TicketPriority.Medium}>Medium</option>
                    <option value={TicketPriority.High}>High</option>
                    <option value={TicketPriority.Critical}>Critical</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">
                    Estimated Cost (SEK)
                  </label>
                  <input
                    type="number"
                    min="0"
                    step="50"
                    value={estimatedCost}
                    onChange={(e) => setEstimatedCost(Number(e.target.value))}
                    className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>
              </div>

              {/* Modal Buttons */}
              <div className="flex items-center justify-end gap-3 pt-4 border-t border-slate-100">
                <button
                  type="button"
                  onClick={closeCreateModal}
                  className="px-4 py-2 text-sm font-medium text-slate-600 hover:text-slate-800 hover:bg-slate-100 rounded-lg transition"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="inline-flex items-center gap-1.5 px-4 py-2 text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 rounded-lg shadow-sm transition"
                >
                  {submitting ? 'Saving...' : 'Create Ticket'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

export default App