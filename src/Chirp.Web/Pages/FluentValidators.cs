using FluentValidation;

namespace Chirp.Web.BindableClasses
{   
    // Validators are used to validate the data before it is sent to the server
    public class EmailValidator : AbstractValidator<NewEmail>
    {
        public EmailValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
        }
    }
}