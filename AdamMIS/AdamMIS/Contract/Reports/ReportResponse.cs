namespace AdamMIS.Contract.Reports
{
    public class ReportResponse
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        //public long FileSize { get; set; }
        //public string FileSizeFormatted => FormatFileSize(FileSize);
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        //private static string FormatFileSize(long bytes)
        //{
        //    string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        //    int counter = 0;
        //    decimal number = bytes;

        //    while (Math.Round(number / 1024) >= 1)
        //    {
        //        number /= 1024;
        //        counter++;
        //    }

        //    return $"{number:n1} {suffixes[counter]}";
        //}
    }
}
