using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ServiceFlow.Core.Constants;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null )
        {
            return NotFound($"Employee with ID {id} was not found.");
        }

        return Ok(employee);
    }

    // POST: api/employees
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        
        var employee = new Employee (
            request.FullName,
            request.Email,
            request.Department
        );
        
        await _employeeService.AddAsync(employee);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);

    }
}