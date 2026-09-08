using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Services;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: api/customers
    [HttpGet]
    public IActionResult GetAll()
    {
        var customers = _customerService.GetAll();
        return Ok(customers);
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var customers = _customerService.GetById(id);
        if(customers == null)
        {
            return NotFound($"Customer with ID {id} was not found.");
        }
        return Ok(customers);
    }


    // POST: api/customers
    [HttpPost]
    public IActionResult Create([FromBody] CreateCustomerRequest request)
    {
        
        var customer = new Customer(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.CompanyName
        );
    
        _customerService.Add(customer);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }
}