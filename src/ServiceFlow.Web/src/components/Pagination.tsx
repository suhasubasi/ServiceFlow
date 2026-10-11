import { ChevronLeft, ChevronRight } from 'lucide-react'

interface PaginationProps {
    page: number
    totalPages: number
    onChange: (page: number) => void
}

function Pagination({page, totalPages, onChange}: PaginationProps){
    if (totalPages <= 1) return null

    return (
        <div className="flex items-center justify-center gap-3 mt-8">
            <button
                onClick={() => onChange(page - 1)}
                disabled={page === 1}
                className="inline-flex items-center gap-1 text-sm font-semibold text-slate-700 bg-white border border-slate-300 hover:bg-slate-50 px-3 py-1.5 rounded-lg shadow-sm transition disabled:opacity-40 disabled:cursor-not-allowed"
            >
                <ChevronLeft className="w-4 h-4" />
                Previous    
            </button>
            <span className="text-sm text-slate-500">
                Page {page} of {totalPages}
            </span>
            <button
                onClick={() => onChange(page + 1)}
                disabled={page === totalPages}
                className="inline-flex items-center gap-1 text-sm font-semibold text-slate-700 bg-white border border-slate-300 hover:bg-slate-50 px-3 py-1.5 rounded-lg shadow-sm transition disabled:opacity-40 disabled:cursor-not-allowed"
            >
                Next
                <ChevronRight className="w-4 h-4" />
            </button>
        </div>
    )
}

export default Pagination