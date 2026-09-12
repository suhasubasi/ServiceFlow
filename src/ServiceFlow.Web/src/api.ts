import type {
    ServiceTicket,
    Customer,
    Employee,
    CreateTicketRequest,
} from './types'

const API_BASE_URL = 'http://localhost:5021/api'

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
        headers: { 'Content-Type': 'application/json' },
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
    })
    if (!response.ok) {
        throw new Error('Failed to delete ticket')
    }
}