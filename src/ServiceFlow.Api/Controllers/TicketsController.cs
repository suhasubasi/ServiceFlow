using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IEmployeeService _employeeService;

    public TicketsController(ITicketService ticketService, IEmployeeService employeeService)
    {
        _ticketService = ticketService;
        _employeeService = employeeService;
    }

    // GET: api/tickets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();
        return Ok(tickets);
    }

    // GET: api/tickets/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        return Ok(ticket);
    }

    // GET: api/tickets/customer/{customerId}
    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomerId(string customerId)
    {
        var tickets = await _ticketService.GetByCustomerIdAsync(customerId);
        return Ok(tickets);
    }

    // GET: api/tickets/employee/{employeeId}
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployeeId(Guid employeeId)
    {
        var tickets = await _ticketService.GetByEmployeeIdAsync(employeeId);
        return Ok(tickets);
    }


    // POST: api/tickets
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
    {
        // 1. Convert DTO to Domain Entity
        var ticket = new ServiceTicket(
            request.Title,
            request.Description,
            request.CustomerId,
            request.Priority,
            new Money(request.EstimatedCostAmount)
        );

        // 2. Save into our in-memory service
        await _ticketService.AddAsync(ticket);

        // 3. Return 201 Created with link to GetById
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id},ticket);
    }


    // PUT: api/tickets/{id}/assign
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTicketRequest request)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        var employee = await _employeeService.GetByIdAsync(request.EmployeeId);
        if (employee == null)
        {
            return NotFound($"Employee with ID {request.EmployeeId} was not found.");
        }

        ticket.Assign(employee);
        await _ticketService.UpdateAsync();
        return Ok(ticket);
        
    }

    // PUT: api/tickets/{id}/resolve
    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        ticket.Resolve();
        await _ticketService.UpdateAsync();
        return Ok(ticket);
    }

    // PUT: api/tickets/{id}/close
    [HttpPut("{id}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        ticket.Close();
        await _ticketService.UpdateAsync();
        return Ok(ticket);
    }


    // DELETE: api/tickets/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // 1. Call the service, this returns a bool: true or false
        bool removed = await _ticketService.RemoveAsync(id);

        // 2. If it returned false, tell the client HTTP 404 (Not Found)
        if (!removed)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        // 3. If it returned true, tell the client HTTP 204 (No Content)
        return NoContent();



    }


}