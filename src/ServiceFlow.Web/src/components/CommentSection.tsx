import { useState } from "react";
import { MessageSquare } from "lucide-react";
import type { TicketComment } from "../types";
import { getComments, addComment } from "../api";

interface CommentSectionProps {
    ticketId: string 
    isLoggedIn: boolean
}

function CommentSection({ ticketId, isLoggedIn }: CommentSectionProps) {
    const [open, setOpen] = useState(false)
    const [comments, setComments] = useState<TicketComment[]>([])
    const [text, setText] = useState('')
    const [error, setError] = useState<string | null>(null)

    async function toggle() {
        if (open) {
            setOpen(false)
            return
        }
        setOpen(true)
        try {
            setComments(await getComments(ticketId))
            setError(null)
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Something went wrong')
        }
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault()
        if (!text.trim()) return
        try {
            const newComment = await addComment(ticketId, text)
            setComments((prev) => [...prev, newComment])
            setText('')
            setError(null)
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Something went wrong')
        }
    }

    return (
    <div className="text-xs">
      <button
        onClick={toggle}
        className="inline-flex items-center gap-1.5 text-slate-500 hover:text-slate-800 transition"
      >
        <MessageSquare className="w-3.5 h-3.5" />
        {open ? 'Hide comments' : 'Comments'}
      </button>
      {open && (
        <div className="mt-2 space-y-2">
          {error && <p className="text-rose-600">{error}</p>}
          {comments.length === 0 && !error && (
            <p className="text-slate-400">No comments yet.</p>
          )}
          {comments.map((comment) => (
            <div key={comment.id} className="bg-slate-50 rounded-lg px-3 py-2">
              <p className="text-slate-800">{comment.text}</p>
              <p className="text-slate-400 mt-1">
                {comment.author} · {new Date(comment.createdAt).toLocaleString('sv-SE')}
              </p>
            </div>
          ))}
          {isLoggedIn && (
            <form onSubmit={handleSubmit} className="flex gap-2">
              <input
                value={text}
                onChange={(e) => setText(e.target.value)}
                maxLength={1000}
                placeholder="Write a comment..."
                className="flex-1 border border-slate-300 rounded-lg px-2 py-1.5 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <button
                type="submit"
                disabled={!text.trim()}
                className="font-semibold bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white py-1.5 px-3 rounded-lg transition"
              >
                Add
              </button>
            </form>
          )}
        </div>
      )}
    </div>
  )
}
export default CommentSection