import { useState } from 'react'
import { X } from 'lucide-react'
import { login } from '../api'

interface LoginModalProps {
    onClose: () => void
    onLoggedIn: () => void
}

function LoginModal({ onClose, onLoggedIn }: LoginModalProps) {
    const [username, setUsername] = useState('admin')
    const [password, setPassword] = useState('admin123')
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        try {
            setSubmitting(true)
            setError(null)
            await login(username.trim(), password)
            onLoggedIn()
            onClose()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Login failed')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <div className="fixed inset-0 bg-slate-900/50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-xl shadow-xl w-full max-w-md max-h-[90vh] overflow-y-auto">
                <div className="flex items-center justify-between px-6 py-4 border-b border-slate-200">
                    <h2 className="text-lg font-semibold text-slate-900">Log in</h2>
                    <button
                        type="button"
                        onClick={onClose}
                        className="text-slate-400 hover:text-slate-600 transition"
                    >
                        <X className="w-5 h-5" />
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="px-6 py-5 space-y-4">
                    {error && (
                        <div className="text-sm text-red-700 bg-red-50 border border-red-200 rounded-lg px-3.5 py-2">
                            {error}
                        </div>
                    )}

                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">Username</label>
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                            className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">Password</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            className="w-full px-3.5 py-2 text-sm border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={submitting}
                        className="w-full text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 px-3.5 py-2 rounded-lg transition"
                    >
                        {submitting ? 'Logging in...' : 'Log in'}
                    </button>
                </form>
            </div>
        </div>
    )
}

export default LoginModal