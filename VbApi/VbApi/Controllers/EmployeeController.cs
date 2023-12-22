using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace VbApi.Controllers;

public class Employee 
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }

    public double HourlySalary { get; set; }
}

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(employee => employee.Name)
            .NotEmpty()
            .Length(10, 250)
            .WithMessage("Invalid Name");

        RuleFor(employee => employee.DateOfBirth)
            .NotEmpty()
            .Must(dateOfBirth => DateTime.Today.AddYears(-65) <= dateOfBirth)
            .WithMessage("Birthdate is not valid.");

        RuleFor(employee => employee.Email)
            .EmailAddress()
            .WithMessage("Email address is not valid.");

        RuleFor(employee => employee.Phone)
            .Matches(@"^\(?\d{1,4}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,9}$")
            .WithMessage("Phone is not valid.");

        RuleFor(employee => employee.HourlySalary)
            .InclusiveBetween(50, 400)
            .WithMessage("Hourly salary does not fall within allowed range.")
            .Must((employee, hourlySalary) => 
                    (employee.DateOfBirth <= DateTime.Today.AddYears(-30) && hourlySalary >= 200) ||
                    (employee.DateOfBirth > DateTime.Today.AddYears(-30) && hourlySalary >= 50))
            .WithMessage("Minimum hourly salary is not valid.");
        }
    }

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IValidator<Employee> _validator;

    public EmployeeController(IValidator<Employee> validator)
    {
       _validator = validator;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Employee value)
    {
        var validationResult = _validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
 
        return Ok(value);
    }
}