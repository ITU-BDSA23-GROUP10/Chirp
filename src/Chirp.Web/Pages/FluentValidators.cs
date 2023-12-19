using FluentValidation;

namespace Chirp.Web.BindableClasses
{
    public class EmailValidator : AbstractValidator<NewEmail>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}