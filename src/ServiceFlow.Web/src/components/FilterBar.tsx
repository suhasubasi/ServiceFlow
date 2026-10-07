import { Wrench, CheckCircle2, Clock, Archive } from 'lucide-react'
import type { ReactNode } from 'react'
import { TicketStatus } from '../types'

export type StatusFilter = 'all' | TicketStatus

export interface StatusCounts {
  all: number
  open: number
  inProgress: number
  resolved: number
  closed: number
}

interface FilterBarProps {
  statusFilter: StatusFilter
  counts: StatusCounts
  onChange: (filter: StatusFilter) => void
}

interface FilterOption {
  value: StatusFilter
  label: string
  count: number
  icon?: ReactNode
  activeButton: string
  activeCount: string
}

function FilterBar({ statusFilter, counts, onChange }: FilterBarProps) {
  const options: FilterOption[] = [
    { value: 'all', label: 'All', count: counts.all, activeButton: 'bg-blue-600', activeCount: 'bg-blue-700' },
    {
      value: TicketStatus.Open,
      label: 'Open',
      count: counts.open,
      icon: <Clock className="w-3.5 h-3.5" />,
      activeButton: 'bg-amber-500',
      activeCount: 'bg-amber-600',
    },
    {
      value: TicketStatus.InProgress,
      label: 'In Progress',
      count: counts.inProgress,
      icon: <Wrench className="w-3.5 h-3.5" />,
      activeButton: 'bg-blue-600',
      activeCount: 'bg-blue-700',
    },
    {
      value: TicketStatus.Resolved,
      label: 'Resolved',
      count: counts.resolved,
      icon: <CheckCircle2 className="w-3.5 h-3.5" />,
      activeButton: 'bg-emerald-600',
      activeCount: 'bg-emerald-700',
    },
    {
      value: TicketStatus.Closed,
      label: 'Closed',
      count: counts.closed,
      icon: <Archive className="w-3.5 h-3.5" />,
      activeButton: 'bg-slate-700',
      activeCount: 'bg-slate-800',
    },
  ]

  return (
    <div className="flex flex-wrap items-center gap-2 mb-6">
      {options.map((option) => {
        const isActive = statusFilter === option.value
        return (
          <button
            key={option.label}
            onClick={() => onChange(option.value)}
            className={`inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full text-xs font-semibold transition ${
              isActive
                ? `${option.activeButton} text-white shadow-sm`
                : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-100'
            }`}
          >
            {option.icon}
            {option.label}
            <span
              className={`px-1.5 py-0.5 rounded-full text-[11px] ${
                isActive ? `${option.activeCount} text-white` : 'bg-slate-100 text-slate-600'
              }`}
            >
              {option.count}
            </span>
          </button>
        )
      })}
    </div>
  )
}

export default FilterBar

