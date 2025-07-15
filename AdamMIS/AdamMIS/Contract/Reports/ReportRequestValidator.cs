namespace AdamMIS.Contract.Reports
{
    public class ReportRequestValidator : AbstractValidator<ReportRequest>
    {
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

        public ReportRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("Please select a file to upload")
                .Must(BeValidFile)
                .WithMessage("Invalid file provided");

            RuleFor(x => x.File)
                .Must(HaveValidExtension)
                .WithMessage($"File must have one of the following extensions: {string.Join(", ", _allowedExtensions)}")
                .When(x => x.File != null);

            RuleFor(x => x.File)
                .Must(BeWithinSizeLimit)
                .WithMessage($"File size must not exceed {_maxFileSize / (1024 * 1024)} MB")
                .When(x => x.File != null);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Please select a valid category");
        }

        private bool BeValidFile(IFormFile file)
        {
            return file != null && file.Length > 0;
        }

        private bool HaveValidExtension(IFormFile file)
        {
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private bool BeWithinSizeLimit(IFormFile file)
        {
            if (file == null) return false;

            return file.Length <= _maxFileSize;
        }
    }
}
