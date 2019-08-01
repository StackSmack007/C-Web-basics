namespace TorshiaApp.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class TaskSector
    {
        public int TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public virtual Task Task { get; set; }

        public int SectorId { get; set; }
        [ForeignKey(nameof(SectorId))]
        public virtual Sector Sector { get; set; }
    }
}