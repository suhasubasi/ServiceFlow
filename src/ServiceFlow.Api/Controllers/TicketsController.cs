using System.Data.SqlTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Services;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly EmployeeService _employeeService;

    public TicketsController(TicketService ticketService, EmployeeService employeeService)
    {
        _ticketService = ticketService;
        _employeeService = employeeService;
    }

    // GET: api/tickets
    [HttpGet]
    public IActionResult GetAll()
    {
        var tickets = _ticketService.GetAll();
        return Ok(tickets);
    }

    // GET: api/tickets/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var ticket = _ticketService.GetById(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        return Ok(ticket);
    }

    // GET: api/tickets/customer/{customerId}
    [HttpGet("customer/{customerId}")]
    public IActionResult GetByCustomerId(string customerId)
    {
        var tickets = _ticketService.GetByCustomerId(customerId);
        return Ok(tickets);
    }

    // GET: api/tickets/employee/{employeeId}
    [HttpGet("employee/{employeeId}")]
    public IActionResult GetByEmployeeId(Guid employeeId)
    {
        var tickets = _ticketService.GetByEmployeeId(employeeId);
        return Ok(tickets);
    }


    // POST: api/tickets
    [HttpPost]
    public IActionResult Create([FromBody] CreateTicketRequest request)
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
        _ticketService.Add(ticket);

        // 3. Return 201 Created with link to GetById
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id},ticket);
    }


    // PUT: api/tickets/{id}/assign
    [HttpPut("{id}/assign")]
    public IActionResult Assign(Guid id, [FromBody] AssignTicketRequest request)
    {
        var ticket = _ticketService.GetById(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        var employee = _employeeService.GetById(request.EmployeeId);
        if (employee == null)
        {
            return NotFound($"Employee with ID {request.EmployeeId} was not found.");
        }

        ticket.Assign(employee);
        return Ok(ticket);
        
    }

    // PUT: api/tickets/{id}/resolve
    [HttpPut("{id}/resolve")]
    public IActionResult Resolve(Guid id)
    {
        var ticket = _ticketService.GetById(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        ticket.Resolve();
        return Ok(ticket);
    }

    // PUT: api/tickets/{id}/close
    [HttpPut("{id}/close")]
    public IActionResult Close(Guid id)
    {
        var ticket = _ticketService.GetById(id);
        if (ticket == null)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        ticket.Close();
        return Ok(ticket);
    }


    // DELETE: api/tickets/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        // 1. Call the service, this returns a bool: true or false
        bool removed = _ticketService.Remove(id);

        // 2. If it returned false, tell the client HTTP 404 (Not Found)
        if (!removed)
        {
            return NotFound($"Ticket with ID {id} was not found.");
        }

        // 3. If it returned true, tell the client HTTP 204 (No Content)
        return NoContent();



    }


}