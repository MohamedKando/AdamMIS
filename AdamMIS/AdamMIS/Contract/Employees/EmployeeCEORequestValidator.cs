namespace AdamMIS.Contract.Employees
{
    public class EmployeeCEORequestValidator : AbstractValidator<EmployeeCEORequest>
    {
        public EmployeeCEORequestValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
        }
    }
}
