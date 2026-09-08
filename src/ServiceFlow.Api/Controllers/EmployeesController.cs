using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Api.DTOs;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Services;

namespace ServiceFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public EmployeesController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public IActionResult GetAll()
    {
        var employees = _employeeService.GetAll();
        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var employee = _employeeService.GetById(id);
        if (employee == null )
        {
            return NotFound($"Employee with ID {id} was not found.");
        }

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public IActionResult Create([FromBody] CreateEmployeeRequest request)
    {
        
        var employee = new Employee (
            request.FullName,
            request.Email,
            request.Department
        );
        
        _employeeService.Add(employee);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);

    }
}