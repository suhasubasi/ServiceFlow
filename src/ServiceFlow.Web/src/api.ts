import type {
    ServiceTicket,
    Customer,
    Employee,
    CreateTicketRequest,
    CreateCustomerRequest,
    CreateEmployeeRequest,
} from './types'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5021/api'

const TOKEN_KEY = 'serviceflow_token'
const ROLE_KEY = 'serviceflow_role'

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token)
}

export function clearToken(): void {
    localStorage.removeItem(TOKEN_KEY)
}

export function getRole(): string | null {
    return localStorage.getItem(ROLE_KEY)
}

function authHeaders(extra: Record<string, string> = {}): HeadersInit {
    const headers: Record<string, string> = { ...extra}
    const token = getToken()
    if (token) {
        headers['Authorization'] = `Bearer ${token}`
    }
    return headers
}

export async function login(username: string, password: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({username, password}),
    })
    if (!response.ok) {
        throw new Error('Wrong credentials')
    }
    const data = await response.json()
    setToken(data.token)
    localStorage.setItem(ROLE_KEY, data.role)
}

export function logout(): void {
    clearToken()
    localStorage.removeItem(ROLE_KEY)
}

// Reads the error messages from a 400 validation response
async function getErrorMessage(response: Response, fallback: string): Promise<string> {
    try {
        const data = await response.json()
        if (data.errors) {
            return Object.values(data.errors).flat().join(' ')
        }
    } catch {

    }
    return fallback
}

// 1. Fetch all tickets from GET /api/tickets
export async function getTickets(): Promise<ServiceTicket[]> {
    const response = await fetch(`${API_BASE_URL}/tickets`)
    if(!response.ok){
        throw new Error('Failed to fetch tickets from API')
    }
    return response.json()
}

// 2. Fetch all customers from GET /api/customers
export async function getCustomers(): Promise<Customer[]> {
    const response = await fetch(`${API_BASE_URL}/customers`)
    if (!response.ok){
        throw new Error('Failed to fetch customers')
    }
    return response.json();
}

// 3. Fetch all employees from GET /api/employees
export async function getEmployees(): Promise<Employee[]> {
    const response = await fetch(`${API_BASE_URL}/employees`)
    if (!response.ok){
        throw new Error('Failed to fetch employees')
    }
    return response.json();
}

// 4. Create a new ticket via POST /api/tickets
export async function createTicket(ticket: CreateTicketRequest): Promise<ServiceTicket> {
    const response = await fetch(`${API_BASE_URL}/tickets`, {
        method: 'POST', 
        headers: authHeaders({ 'Content-Type': 'application/json' }),
        body: JSON.stringify(ticket),
        })
    if (!response.ok){
        throw new Error('Failed to create ticket')
    }
    return response.json()
}

// 5. Resolve a ticket via PUT /api/tickets/{id}/resolve
export async function resolveTicket(id: string): Promise<ServiceTicket> {
    const response = await fetch(`${API_BASE_URL}/tickets/${id}/resolve`, {
        method: 'PUT',
        headers: authHeaders(),
    })
    if(!response.ok){
        throw new Error('Failed to resolve ticket')
    }
    return response.json()
}

// 6. Close a ticket via PUT /api/tickets/{id}/close
export async function closeTicket(id: string): Promise<ServiceTicket> {
    const response = await fetch(`${API_BASE_URL}/tickets/${id}/close`, {
        method: 'PUT',
        headers: authHeaders(),
    })
    if (!response.ok) {
        throw new Error('Failed to close ticket')
    }
    return response.json()
}

// 7. Delete a ticket via DELETE /api/tickets/{id}
export async function deleteTicket(id: string): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/tickets/${id}`, {
        method: 'DELETE',
        headers: authHeaders(),
    })
    if (!response.ok) {
        throw new Error('Failed to delete ticket')
    }
}

// 8. Assign a ticket to an employee via PUT /api/tickets/{id}/assign
export async function assignTicket(id: string, employeeId: string): Promise<ServiceTicket> {
    const response = await fetch(`${API_BASE_URL}/tickets/${id}/assign`, {
        method: 'PUT',
        headers: authHeaders({'Content-Type': 'application/json'}),
        body: JSON.stringify({employeeId}),
    })
    if (!response.ok){
        throw new Error('Failed to assign ticket')
    }
    return response.json()
}

// 9. Create a new customer via POST /api/customers
export async function createCustomer(customer: CreateCustomerRequest): Promise<Customer> {
    const response = await fetch(`${API_BASE_URL}/customers`, {
        method: 'POST',
        headers: authHeaders({ 'Content-Type': 'application/json' }),
        body: JSON.stringify(customer),
    })
    if (!response.ok) {
        throw new Error(await getErrorMessage(response, 'Failed to create customer'))
    }
    return response.json()
}

// 10. Create a new employee via POST /api/employees
export async function createEmployee(employee: CreateEmployeeRequest): Promise<Employee> {
    const response = await fetch(`${API_BASE_URL}/employees`, {
        method: 'POST',
        headers: authHeaders({ 'Content-Type': 'application/json'}),
        body: JSON.stringify(employee),
    })
    if (!response.ok) {
        throw new Error(await getErrorMessage(response, 'Failed to create employee'))
    }
    return response.json()
}