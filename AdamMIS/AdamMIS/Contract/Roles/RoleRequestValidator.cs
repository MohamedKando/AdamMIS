namespace AdamMIS.Contract.Roles
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3,50);

            RuleFor(x => x.Permissions).NotNull().NotEmpty();

            RuleFor(x => x.Permissions).Must(x => x.Distinct().Count() == x.Count()).WithMessage("you cant add dublicated permission for the same role")
                .When(x=>x.Permissions!=null);
            
        }
    }
}
