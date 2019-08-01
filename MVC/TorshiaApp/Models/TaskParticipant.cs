namespace TorshiaApp.Models
{
using System.ComponentModel.DataAnnotations.Schema;
    public class TaskParticipant
    {
        public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        public int ParticipantId { get; set; }
        [ForeignKey(nameof(ParticipantId))]
        public virtual Participant Participant { get; set; }
    }
}