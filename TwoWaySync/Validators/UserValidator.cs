using Domain.User;
using FluentValidation;

namespace TwoWaySync.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Id).NotEmpty().NotNull().GreaterThan(0);
    }
}
