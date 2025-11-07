using FluentValidation;
using BackendAspNetCore.RequestBody.Sensor;

namespace BackendAspNetCore.Validators.Sensor;

public class GetAllSensorRequestBodyValidator : AbstractValidator<GetAllSensorRequestBody>
{
    public GetAllSensorRequestBodyValidator()
    {
        RuleFor(x => x.Limit)
            .NotNull().WithMessage("Limit is required.")
            .GreaterThan(0).WithMessage("Limit must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Limit must not exceed 100.");
    }
}
