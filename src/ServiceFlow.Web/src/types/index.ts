// 1. Enums matching our C# enums (Modern TypeScript pattern)
export const TicketStatus = {
  Open: 0,
  InProgress: 1,
  Resolved: 2,
  Closed: 3,
} as const
export type TicketStatus = (typeof TicketStatus)[keyof typeof TicketStatus]

export const TicketPriority = {
  Low: 0,
  Medium: 1,
  High: 2,
  Critical: 3,
} as const
export type TicketPriority = (typeof TicketPriority)[keyof typeof TicketPriority]

// 2. Value Object matching C# Money record struct
export interface Money {
  amount: number
  currency: string
}

// 3. Main Entity matching C# ServiceTicket
export interface ServiceTicket {
  id: string
  assignedEmployeeId?: string | null
  title: string
  description: string
  customerId: string
  assignedTo: string
  priority: TicketPriority
  status: TicketStatus
  estimatedCost: Money
  createdAt: string
}

// 4. Entity matching C# Customer
export interface Customer {
  id: string
  fullName: string
  email: string
  phoneNumber: string
  companyName: string
  createdAt: string
}

// 5. Entity matching C# Employee
export interface Employee {
  id: string
  fullName: string
  email: string
  department: string
  createdAt: string
}

// 6. DTO for creating a new ticket (matches C# CreateTicketRequest)
export interface CreateTicketRequest {
  title: string
  description: string
  customerId: string
  priority: TicketPriority
  estimatedCostAmount: number
}