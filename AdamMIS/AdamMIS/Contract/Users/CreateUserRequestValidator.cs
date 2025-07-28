namespace AdamMIS.Contract.Users
{
    public class CreateUserRequestValidator : AbstractValidator <CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Password).NotEmpty().Matches(RegexPatterns.Password).WithMessage("Password should at least 4 digits");
            RuleFor(x => x.UserName).NotEmpty().Length(2, 15);
            RuleFor(x => x.Roles).NotEmpty();
        }
    }
}
