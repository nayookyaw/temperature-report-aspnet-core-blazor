using FluentValidation;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Validators.Sensor;

public class ListSensorRequestBodyValidator : AbstractValidator<ListSensorRequestBody>
{
    public ListSensorRequestBodyValidator()
    {
        RuleFor(x => x.Page)
            .NotNull().WithMessage("Page is required.")
            .GreaterThan(0).WithMessage("Page must be greater than 0.");
        RuleFor(x => x.PageSize)
            .NotNull().WithMessage("Page Size is required.")
            .GreaterThan(0).WithMessage("Page Size must be greater than 0.");
    }
}
