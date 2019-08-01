namespace TorshiaApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using TorshiaApp.Models.Enumerations;
    public class Report : BaseEntity<int>
    {
        public ReportStatus Status { get; set; }
        public DateTime ReportedOn { get; set; }

        public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        public int reporterId { get; set; }
        [ForeignKey(nameof(reporterId))]
        public virtual User Reporter { get; set; }
    }
}