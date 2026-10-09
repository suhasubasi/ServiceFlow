import { useState } from 'react'
import { Wrench, CheckCircle2, Clock, Trash2, Check, Archive } from 'lucide-react'
import type { ServiceTicket, Employee } from '../types'
import { TicketStatus, TicketPriority } from '../types'

interface TicketCardProps {
  ticket: ServiceTicket
  employees: Employee[]
  canDelete: boolean // NEW: only admins can delete
  onAssign: (ticketId: string, employeeId: string) => void
  onResolve: (ticketId: string) => void
  onClose: (ticketId: string) => void
  onDelete: (ticketId: string) => void
}

// Helper for Status Badge styling
function getStatusBadge(status: number) {
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
function getPriorityBadge(priority: number) {
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

// NEW: canDelete added to the destructured props
function TicketCard({ ticket, employees, canDelete, onAssign, onResolve, onClose, onDelete }: TicketCardProps) {
  // Which employee is selected in this card's dropdown
  const [selectedEmployeeId, setSelectedEmployeeId] = useState('')

  const isFinished = ticket.status === TicketStatus.Resolved || ticket.status === TicketStatus.Closed

  return (
    <div className="bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition p-5 flex flex-col justify-between">
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

        {/* Assign Technician (only for open and in-progress tickets) */}
        {!isFinished && (
          <div className="flex items-center gap-2">
            <select
              value={selectedEmployeeId}
              onChange={(e) => setSelectedEmployeeId(e.target.value)}
              className="flex-1 text-xs border border-slate-300 rounded-lg px-2 py-1.5 bg-white text-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select technician...</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>
                  {employee.fullName}
                </option>
              ))}
            </select>
            <button
              onClick={() => onAssign(ticket.id, selectedEmployeeId)}
              disabled={!selectedEmployeeId}
              className="text-xs font-semibold bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white py-1.5 px-3 rounded-lg transition"
            >
              Assign
            </button>
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex items-center gap-2 pt-1">
          {!isFinished && (
            <button
              onClick={() => onResolve(ticket.id)}
              className="flex-1 inline-flex items-center justify-center gap-1.5 text-xs font-semibold bg-emerald-600 hover:bg-emerald-700 text-white py-1.5 px-3 rounded-lg transition"
            >
              <Check className="w-3.5 h-3.5" />
              Resolve
            </button>
          )}

          {ticket.status !== TicketStatus.Closed && (
            <button
              onClick={() => onClose(ticket.id)}
              className="flex-1 inline-flex items-center justify-center gap-1.5 text-xs font-semibold bg-slate-700 hover:bg-slate-800 text-white py-1.5 px-3 rounded-lg transition"
            >
              <Archive className="w-3.5 h-3.5" />
              Close
            </button>
          )}

          {/* NEW: Delete is only shown to admins */}
          {canDelete && (
            <button
              onClick={() => onDelete(ticket.id)}
              className="p-1.5 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
              title="Delete Ticket"
            >
              <Trash2 className="w-4 h-4" />
            </button>
          )}
        </div>
      </div>
    </div>
  )
}

export default TicketCard