namespace TorshiaApp.Controllers
{
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Linq;

    using TorshiaApp.DTO.Tasks;
    using TorshiaApp.Models;
    using TorshiaApp.Models.Enumerations;

    public class TaskController : BaseController
    {
        [Authorised]
        public IHttpResponse Create()
        {
            if (GetRole() != UserRole.Admin)
            {
                return MessageError("Only Admins can add tasks");
            }
        return View();
        }

        [Authorised]
        [HttpPost]
        public IHttpResponse Create(CreateTaskDTO dto)
        {
            if (DB.Tasks.Any(x=>x.Title==dto.TaskName))
            {
                return MessageWithView("Task already exists in Database");
            }
            if (GetRole() != UserRole.Admin)
            {
                return MessageError("Only Admins can add tasks");
            }
            Task newTask = new Task
            {
                Title = dto.TaskName,
                DueDate = dto.DueDate,
                Description = dto.Description,
            };

            foreach (string name in dto.Participants)
            {
                var participant = DB.Participants.FirstOrDefault(x => x.Name == name);
                if (participant is null)
                {
                    participant = new Participant { Name = name };
                }
                newTask.TaskParticipants.Add(new TaskParticipant { Participant = participant });
            }
            foreach (string field in dto.Sectors)
            {
                var sector = DB.Sectors.FirstOrDefault(x => x.Name == field);
                if (sector is null)
                {
                    sector = new Sector { Name = field };
                }
                newTask.AffectedSectors.Add(new TaskSector { Sector = sector });
            }
            DB.Tasks.Add(newTask);
            DB.SaveChanges();
            return RedirectResult("/");
        }


    }
}