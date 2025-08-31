namespace AdamMIS.Contract.Employees
{
    public class EmployeeDepartmentHeadRequestValidator : AbstractValidator<EmployeeDepartmentHeadRequest>
    {
        public EmployeeDepartmentHeadRequestValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");

            //RuleFor(x => x.SystemPermissions)
            //    .NotEmpty().WithMessage("System permissions must be defined");
        }
    }
}
