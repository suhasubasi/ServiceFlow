import { useState, useEffect, useCallback } from 'react'
import {
  Wrench,
  AlertCircle,
  RefreshCw,
  Plus,
  UserPlus,
  UserCog,
  LogIn,
  LogOut,
  Search,
} from 'lucide-react'
import type { ServiceTicket, Customer, Employee, CreateTicketRequest, PagedResult } from './types'
import { TicketStatus } from './types'
import {
  getTickets,
  getCustomers,
  getEmployees,
  createTicket,
  assignTicket,
  resolveTicket,
  closeTicket,
  deleteTicket,
  getToken,
  logout,
  getRole,
  setUnauthorizedHandler,
  searchTickets,
} from './api'

import CreateCustomerModal from './components/CreateCustomerModal'
import CreateEmployeeModal from './components/CreateEmployeeModal'
import CreateTicketModal from './components/CreateTicketModal'
import FilterBar from './components/FilterBar'
import type { StatusFilter } from './components/FilterBar'
import LoginModal from './components/LoginModal'
import TicketCard from './components/TicketCard'
import Pagination from './components/Pagination'

const PAGE_SIZE = 6

function App() {
  // 1. Dashboard State
  const [tickets, setTickets] = useState<ServiceTicket[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [statusFilter, setStatusFilter] = useState<StatusFilter>('all')

  // Employees for the assign dropdown on each ticket card
  const [employees, setEmployees] = useState<Employee[]>([])

  // Search and paging for the ticket grid
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [pageResult, setPageResult] = useState<PagedResult<ServiceTicket> | null>(null)

  // Count badges for each filter pill
  const counts = {
    all: tickets.length,
    open: tickets.filter((t) => t.status === TicketStatus.Open).length,
    inProgress: tickets.filter((t) => t.status === TicketStatus.InProgress).length,
    resolved: tickets.filter((t) => t.status === TicketStatus.Resolved).length,
    closed: tickets.filter((t) => t.status === TicketStatus.Closed).length,
  }

  // 2. Modal State
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [customers, setCustomers] = useState<Customer[]>([])
  const [submitting, setSubmitting] = useState(false)
  const [isCustomerModalOpen, setIsCustomerModalOpen] = useState(false)
  const [isEmployeeModalOpen, setIsEmployeeModalOpen] = useState(false)
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false)
  const [isLoggedIn, setIsLoggedIn] = useState(() => !!getToken())
  const [role, setRole] = useState(() => getRole())
  const isAdmin = role === 'Admin'

  // 3. Fetch all tickets from C# API (used for the counts)
  const loadTickets = async () => {
    try {
      const data = await getTickets()
      setTickets(data)
      setError(null)
    } catch (err) {
      console.error('Failed to load tickets', err)
      setError('Could not connect to API. Is ASP.NET Core running on port 5021?')
    } finally {
      setLoading(false)
    }
  }

  // Fetch one page of tickets for the grid
  const loadPage = useCallback(async () => {
    try {
      const status = statusFilter === 'all' ? null : statusFilter
      const data = await searchTickets(search, status, page, PAGE_SIZE)
      setPageResult(data)
    } catch (err) {
      console.error('Failed to search tickets', err)
    }
  }, [search, statusFilter, page])

  // Reload counts and the current page after a change
  const refresh = async () => {
    await Promise.all([loadTickets(), loadPage()])
  }

  // Refresh button: show the spinner, then reload
  const handleRefresh = () => {
    setLoading(true)
    refresh()
  }

  // Load employees for the assign dropdown on each ticket card
  const loadEmployees = async () => {
    try {
      const data = await getEmployees()
      setEmployees(data)
    } catch (err) {
      console.error('Could not load employees for assign dropdown', err)
    }
  }

  // 4. Load Customers for the dropdown, then open the modal
  const openCreateModal = async () => {
    try {
      const customerData = await getCustomers()
      setCustomers(customerData)
    } catch (err) {
      console.error('Could not load customers for dropdown', err)
    }
    setIsModalOpen(true)
  }

  // Load tickets and employees once when the page opens
  useEffect(() => {
    // oxlint-disable-next-line react/set-state-in-effect -- setState runs after await
    loadTickets()
    loadEmployees()
  }, [])

  // When the token expires: show logged out and open the login window
  useEffect(() => {
    setUnauthorizedHandler(() => {
      setIsLoggedIn(false)
      setRole(null)
      setIsLoginModalOpen(true)
    })
  }, [])

  // Wait 300 ms after typing stops, then search
  useEffect(() => {
    const timer = setTimeout(loadPage, 300)
    return () => clearTimeout(timer)
  }, [loadPage])

  // 5. Action: Create Ticket (called by the modal on submit)
  const handleCreateTicket = async (ticket: CreateTicketRequest) => {
    try {
      setSubmitting(true)
      await createTicket(ticket)
      setIsModalOpen(false)
      await refresh()
    } catch (err) {
      console.error('Failed to create ticket', err)
      alert('Failed to create ticket. Make sure the backend API is running.')
    } finally {
      setSubmitting(false)
    }
  }

  // 6. Action: Resolve a ticket
  const handleResolve = async (id: string) => {
    try {
      await resolveTicket(id)
      await refresh()
    } catch (err) {
      console.error('Failed to resolve ticket', err)
      alert('Failed to resolve ticket.')
    }
  }

  // 7. Action: Close a ticket
  const handleClose = async (id: string) => {
    try {
      await closeTicket(id)
      await refresh()
    } catch (err) {
      console.error('Failed to close ticket', err)
      alert('Failed to close ticket.')
    }
  }

  // 8. Action: Delete a ticket
  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this ticket?')) return
    try {
      await deleteTicket(id)
      await refresh()
    } catch (err) {
      console.error('Failed to delete ticket', err)
      alert('Failed to delete ticket.')
    }
  }

  // 9. Action: Assign a technician to a ticket
  const handleAssign = async (ticketId: string, employeeId: string) => {
    try {
      await assignTicket(ticketId, employeeId)
      await refresh()
    } catch (err) {
      console.error('Failed to assign ticket', err)
      alert('Failed to assign ticket.')
    }
  }

  // Clear search and filter, go back to page 1
  const showAllTickets = () => {
    setStatusFilter('all')
    setSearch('')
    setPage(1)
  }

  return (
      <div className="min-h-screen bg-slate-50 text-slate-900">
        {/* Top Navbar */}
        <header className="bg-white border-b border-slate-200 sticky top-0 z-10">
          <div className="max-w-6xl mx-auto px-4 py-4 flex items-center justify-between">
            <button
                type="button"
                onClick={handleRefresh}
                className="flex items-center gap-3 text-left"
                title="Refresh"
            >
              <div className="bg-blue-600 text-white p-2 rounded-lg shadow-sm">
                <Wrench className="w-6 h-6" />
              </div>
              <div>
                <h1 className="text-xl font-bold tracking-tight text-slate-900">ServiceFlow</h1>
                <p className="hidden md:block text-xs text-slate-500">Service Desk & Dispatch Management</p>
              </div>
            </button>
            <div className="flex items-center gap-2 md:gap-3">
              <button
                  onClick={() => setIsCustomerModalOpen(true)}
                  className="inline-flex items-center gap-1.5 text-sm font-semibold text-slate-700 bg-white border border-slate-300 hover:bg-slate-50 px-2.5 md:px-3.5 py-1.5 rounded-lg shadow-sm transition"
              >
                <UserPlus className="w-4 h-4" />
                <span className="hidden md:inline">New Customer</span>
              </button>
              {isAdmin && (
                  <button
                      onClick={() => setIsEmployeeModalOpen(true)}
                      className="inline-flex items-center gap-1.5 text-sm font-semibold text-slate-700 bg-white border border-slate-300 hover:bg-slate-50 px-2.5 md:px-3.5 py-1.5 rounded-lg shadow-sm transition"
                  >
                    <UserCog className="w-4 h-4" />
                    <span className="hidden md:inline">New Employee</span>
                  </button>
              )}
              <button
                  onClick={openCreateModal}
                  className="inline-flex items-center gap-1.5 text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 px-2.5 md:px-3.5 py-1.5 rounded-lg shadow-sm transition"
              >
                <Plus className="w-4 h-4" />
                <span className="hidden md:inline">New Ticket</span>
              </button>
              {isLoggedIn ? (
                  <button
                      onClick={() => {
                        logout()
                        setIsLoggedIn(false)
                        setRole(null)
                      }}
                      className="inline-flex items-center gap-1 text-sm font-semibold text-rose-700 bg-rose-50 border border-rose-200 hover:bg-rose-100 px-2.5 md:px-3.5 py-1.5 rounded-lg shadow-sm transition"
                      title="Log out"
                  >
                    <LogOut className="w-4 h-4" />
                    <span className="md:hidden">Out</span>
                    <span className="hidden md:inline">Log out</span>
                  </button>
              ) : (
                  <button
                      onClick={() => setIsLoginModalOpen(true)}
                      className="inline-flex items-center gap-1 text-sm font-semibold text-white bg-slate-800 hover:bg-slate-900 px-2.5 md:px-3.5 py-1.5 rounded-lg shadow-sm transition"
                      title="Log in"
                  >
                    <LogIn className="w-4 h-4" />
                    <span className="md:hidden">In</span>
                    <span className="hidden md:inline">Log in</span>
                  </button>
              )}
              <button
                  onClick={handleRefresh}
                  className="hidden md:inline-flex items-center gap-2 text-sm text-slate-600 hover:text-slate-900 bg-slate-100 hover:bg-slate-200 px-3 py-1.5 rounded-lg transition"
              >
                <RefreshCw className="w-4 h-4" />
                <span>Refresh</span>
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
                {/* Search Box */}
                <div className="relative mb-4 max-w-md">
                  <Search className="w-4 h-4 text-slate-400 absolute left-3 top-1/2 -translate-y-1/2" />
                  <input
                      type="text"
                      value={search}
                      onChange={(e) => {
                        setSearch(e.target.value)
                        setPage(1)
                      }}
                      placeholder="Search tickets by title or description..."
                      className="w-full pl-9 pr-3 py-2 text-sm bg-white border border-slate-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                {/* Status Filter Pills */}
                <FilterBar
                    statusFilter={statusFilter}
                    counts={counts}
                    onChange={(filter) => {
                      setStatusFilter(filter)
                      setPage(1)
                    }}
                />

                {/* Tickets Grid or Filter Empty State */}
                {!pageResult || pageResult.items.length === 0 ? (
                    <div className="bg-white rounded-xl border border-slate-200 p-8 text-center max-w-sm mx-auto">
                      <p className="text-sm text-slate-500 font-medium mb-3">No tickets match this search or filter.</p>
                      <button
                          onClick={showAllTickets}
                          className="text-xs font-semibold text-blue-600 hover:text-blue-800 underline"
                      >
                        Show all tickets
                      </button>
                    </div>
                ) : (
                    <>
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {pageResult.items.map((ticket) => (
                            <TicketCard
                                key={ticket.id}
                                ticket={ticket}
                                employees={employees}
                                onAssign={handleAssign}
                                onResolve={handleResolve}
                                onClose={handleClose}
                                onDelete={handleDelete}
                                canDelete={isAdmin}
                            />
                        ))}
                      </div>
                      <Pagination
                          page={page}
                          totalPages={pageResult.totalPages}
                          onChange={setPage}
                      />
                    </>
                )}
              </>
          )}
        </main>

        {isModalOpen && (
            <CreateTicketModal
                customers={customers}
                submitting={submitting}
                onClose={() => setIsModalOpen(false)}
                onSubmit={handleCreateTicket}
            />
        )}
        {isCustomerModalOpen && (
            <CreateCustomerModal onClose={() => setIsCustomerModalOpen(false)} />
        )}
        {isEmployeeModalOpen && (
            <CreateEmployeeModal
                onClose={() => setIsEmployeeModalOpen(false)}
                onCreated={loadEmployees}
            />
        )}
        {isLoginModalOpen && (
            <LoginModal
                onClose={() => setIsLoginModalOpen(false)}
                onLoggedIn={() => {
                  setIsLoggedIn(true)
                  setRole(getRole())
                }}
            />
        )}
      </div>
  )
}

export default App