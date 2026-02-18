namespace SK.Report.Models
{
    public class Report
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public string? ReportType { get; set; }
        public string? App { get; set; }
        public bool? IsValid { get; set; }
    }
}
