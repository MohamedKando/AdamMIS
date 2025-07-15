namespace AdamMIS.Contract.Reports
{
    public class UserReportRequestValidator : AbstractValidator<UserReportRequest>
    {
        public UserReportRequestValidator()
        {
            RuleFor(x => x.ReportIds)
                .NotEmpty()
                .WithMessage("Please provide at least one Report ID")
                .Must(reportIds => reportIds.Count > 0)
                .WithMessage("Report IDs list cannot be empty");

            RuleForEach(x => x.ReportIds)
                .GreaterThan(0)
                .WithMessage("Report ID must be greater than 0");

            RuleFor(x => x.UserIds)
                .NotEmpty()
                .WithMessage("Please provide at least one User ID")
                .Must(userIds => userIds.Count > 0)
                .WithMessage("User IDs list cannot be empty");

            RuleForEach(x => x.UserIds)
                .NotEmpty()
                .WithMessage("User ID cannot be empty or null")
                .Length(1, 450) // Adjust based on your User ID max length
                .WithMessage("User ID must be between 1 and 450 characters");

            // Optional: Check for duplicates
            RuleFor(x => x.ReportIds)
                .Must(reportIds => reportIds.Distinct().Count() == reportIds.Count)
                .WithMessage("Duplicate Report IDs are not allowed");

            RuleFor(x => x.UserIds)
                .Must(userIds => userIds.Distinct().Count() == userIds.Count)
                .WithMessage("Duplicate User IDs are not allowed");
        }
    }
}
