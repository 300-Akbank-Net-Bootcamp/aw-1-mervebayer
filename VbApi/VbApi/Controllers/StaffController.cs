using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace VbApi.Controllers;

public class Staff //StaffValidator'da kontrolü sağlandığı için attributelar kaldırıldı.
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? HourlySalary { get; set; }
}

public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(10, 250)
            .WithMessage("Name is not valid.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email address is not valid.");

        RuleFor(x => x.Phone)
            .Matches(@"^\(?\d{1,4}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone is not valid.");

        RuleFor(x => x.HourlySalary)
            .InclusiveBetween(30, 400)
            .When(x => x.HourlySalary.HasValue)
             .WithMessage("HourlySalary is not valid.");
    }
}

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly IValidator<Staff> _validator;

    public StaffController(IValidator<Staff> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Staff value)
    {
        var validationResult = _validator.Validate(value);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        return Ok(value);
    }
}