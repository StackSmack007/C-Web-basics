using System;
using System.Collections.Generic;
using System.Linq;

namespace TorshiaApp.DTO.Tasks
{
    public class CreateTaskDTO
    {
        public string TaskName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string[] Participants { get; set; }
        public ICollection<string> Sectors { get; }

        public CreateTaskDTO(string title, string dueDate, string description, string participants, ICollection<string> sectors)
        {
            Sectors = sectors;
            if (!string.IsNullOrEmpty(dueDate))
            {
                DueDate = DateTime.Parse(dueDate);
            }
            TaskName = title.Replace("+", " ");
            Description = description.Replace("+", " ");
            Participants = participants.Replace("%2C", ",").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("+", " ").Trim()).ToArray();
        }

    }
}
