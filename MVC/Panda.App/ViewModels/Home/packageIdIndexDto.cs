using Panda.Infrastructure.Models.Enums;

namespace Panda.App.ViewModels.Home
{
    public class packageIdIndexDto
    {
        public packageIdIndexDto(int id, string description,Status status)
        {
            Id = id;
            Description = description;
            Status = status.ToString();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}