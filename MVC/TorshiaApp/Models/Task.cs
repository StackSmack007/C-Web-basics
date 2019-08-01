namespace TorshiaApp.Models
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
    public class Task : BaseEntity<int>
    {
        public Task()
        {
            Reports = new HashSet<Report>();
            TaskParticipants = new HashSet<TaskParticipant>();
            AffectedSectors = new HashSet<TaskSector>();
        }

        [Required, MaxLength(64)]
        public string Title { get; set; }

        public DateTime? DueDate { get; set; }

        [NotMapped]
        public bool IsReported => Reports.Any();

        [Required, MaxLength]
        public string Description { get; set; }

        public virtual ICollection<TaskParticipant> TaskParticipants { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

        public virtual ICollection<TaskSector> AffectedSectors { get; set; }

    }
}