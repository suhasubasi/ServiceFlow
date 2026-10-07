import { useState } from 'react'
import { X } from 'lucide-react'
import type { Customer, CreateTicketRequest } from '../types'
import { TicketPriority } from '../types'

interface CreateTicketModalProps {
  customers: Customer[]
  submitting: boolean
  onClose: () => void
  onSubmit: (ticket: CreateTicketRequest) => void
}

function CreateTicketModal({ customers, submitting, onClose, onSubmit }: CreateTicketModalProps) {
  // Form Fields (pre-select the first customer if available)
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [customerId, setCustomerId] = useState(customers[0]?.id ?? '')
  const [priority, setPriority] = useState<TicketPriority>(TicketPriority.Medium)
  const [estimatedCost, setEstimatedCost] = useState<number>(500)

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()

    if (!title.trim()) {
      alert('Please enter a ticket title.')
      return
    }

    if (!customerId) {
      alert('Please select a customer.')
      return
    }

    onSubmit({
      title: title.trim(),
      description: description.trim(),
      customerId: customerId,
      priority: Number(priority) as TicketPriority,
      estimatedCostAmount: Number(estimatedCost) || 0,
    })
  }

  return (
    <div className="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-xl border border-slate-200 w-full max-w-lg max-h-[90vh] overflow-y-auto">
        {/* Modal Header */}
        <div className="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
          <div>
            <h2 className="text-lg font-bold text-slate-900">New Ticket</h2>
            <p className="text-xs text-slate-500">Register a new customer issue or repair request.</p>
          </div>
          <button
            onClick={onClose}
            className="text-slate-400 hover:text-slate-600 p-1 rounded-lg hover:bg-slate-100 transition"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Modal Form */}
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
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
              onClick={onClose}
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
  )
}

export default CreateTicketModal

