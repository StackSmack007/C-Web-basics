namespace TorshiaApp.DTO.Tasks
{
    public  class TaskDetailsDTO
    {
        public string Name            { get; set; }
        public string DueDate             { get; set; }
        public int    Level           { get; set; }
        public string Participants      { get; set; }
        public string AffectedSectors { get; set; }
        public string Description          { get; set; }
    }
}