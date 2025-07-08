

namespace AdamMIS.Contract.Authentications
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
           // RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().Matches(RegexPatterns.Password).WithMessage("Password should at least 8 digits");
            RuleFor(x => x.UserName).NotEmpty().Length(10,15);
            //RuleFor(x => x.FirstName).NotEmpty().Length(3,100);
            //RuleFor(x => x.LastName).NotEmpty().Length(3, 100);
        }
    }
 
}
