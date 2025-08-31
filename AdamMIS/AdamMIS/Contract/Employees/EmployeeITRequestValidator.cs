namespace AdamMIS.Contract.Employees
{
    public class EmployeeITRequestValidator : AbstractValidator<EmployeeITRequest>
    {
        public EmployeeITRequestValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");

            RuleFor(x => x.FilesSharing)
                .NotEmpty().WithMessage("File sharing permission is required")
                .Must(x => new[] { "None", "ReadOnly", "FullControl" }.Contains(x))
                .WithMessage("File sharing must be None, ReadOnly, or FullControl");
        }
    }
}
