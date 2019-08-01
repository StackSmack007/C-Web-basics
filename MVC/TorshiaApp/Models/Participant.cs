namespace TorshiaApp.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Participant:BaseEntity<int>
    {
        public Participant()
        {
            ParticipantTasks = new HashSet<TaskParticipant>();
        }
        [Required, MaxLength(64)]
        public string Name { get; set; }
        public virtual ICollection<TaskParticipant> ParticipantTasks { get; set; }
    }
}