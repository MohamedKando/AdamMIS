namespace AdamMIS.Contract.Reports
{
    public class RCategoryRequestValidator : AbstractValidator<RCategoryRequest>
    {
        public RCategoryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Please provide a category name")
                .Length(2, 100)
                .WithMessage("Category name must be between 2 and 100 characters")
                .Matches("^[a-zA-Z0-9\\s\\-_]+$")
                .WithMessage("Category name can only contain letters, numbers, spaces, hyphens, and underscores");
        }
    }
}
