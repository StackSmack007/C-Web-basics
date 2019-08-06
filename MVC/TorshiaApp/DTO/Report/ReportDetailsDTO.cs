namespace TorshiaApp.DTO.Report
{
 public   class ReportDetailsDTO
    {
        public int ReportId { get; set; }
        public string TaskName { get; set; }
        public string DueDate { get; set; }
        public string ReportedOn { get; set; }
        public string ReporterName { get; set; }
        public int TaskLevel { get; set; }
        public string Status { get; set; }
        public string Participants { get; set; }
        public string AffectedSectors { get; set; }
        public string TaskDescription { get; set; }
    }
}