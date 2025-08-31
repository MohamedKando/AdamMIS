namespace AdamMIS.Contract.Employees
{
    public class EmployeeHRRequestValidator : AbstractValidator<EmployeeHRRequest>
    {
        public EmployeeHRRequestValidator()
        {
            RuleFor(x => x.EmployeeNumber)
                .NotEmpty().WithMessage("Employee number is required")
                .MaximumLength(50);

            RuleFor(x => x.NameArabic)
                .NotEmpty().WithMessage("Arabic name is required")
                .MaximumLength(200);

            RuleFor(x => x.NameEnglish)
                .NotEmpty().WithMessage("English name is required")
                .MaximumLength(200);

            RuleFor(x => x.PersonalEmail)
                .NotEmpty().EmailAddress().WithMessage("Valid email is required");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Department must be selected");
        }
    }
}
