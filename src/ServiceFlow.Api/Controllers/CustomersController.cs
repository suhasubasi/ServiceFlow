using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customers = await _customerService.GetByIdAsync(id);
        if(customers == null)
        {
            return NotFound($"Customer with ID {id} was not found.");
        }

        return Ok(customers);
    }


    // POST: api/customers
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        
        var customer = new Customer(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.CompanyName
        );
    
        await _customerService.AddAsync(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }
}