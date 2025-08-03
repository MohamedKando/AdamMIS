namespace AdamMIS.Contract.Users
{
    public class UserChangePasswordRequestValidator : AbstractValidator<UserChangePasswordRequest>
    {
        public UserChangePasswordRequestValidator()
        {
            // RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.OldPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should at least 4 digits")
                .NotEqual(x=>x.OldPassword).WithMessage("New Password cannot equal old password");
            RuleFor(x=>x.ConfirmNewPassword).NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should at least 4 digits")
                .Equal(x=>x.NewPassword)
                .WithMessage("New Password and Confirm Password dosent matches");
            //RuleFor(x => x.FirstName).NotEmpty().Length(3,100);
            //RuleFor(x => x.LastName).NotEmpty().Length(3, 100);
        }

    }
}
