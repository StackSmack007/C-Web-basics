namespace TorshiaApp.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Sector : BaseEntity<int>
    {
        public Sector()
        {
            SectorTasks = new HashSet<TaskSector>();
        }

        [Required, MaxLength(16)]
        public string Name { get; set; }

        public virtual ICollection<TaskSector> SectorTasks { get; set; }
    }
}