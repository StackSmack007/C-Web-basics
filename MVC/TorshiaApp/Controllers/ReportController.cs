namespace TorshiaApp.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System;
    using System.Globalization;
    using System.Linq;
    using TorshiaApp.DTO.Report;
    using TorshiaApp.Models;
    using TorshiaApp.Models.Enumerations;
    public class ReportController : BaseController
    {
        [Authorised]
        public IHttpResponse DisplayAll()
        {
            if (GetRole() != UserRole.Admin)
            {
                return MessageError("Only Admins can view Reports");
            }

            ReportDTO[] reports = DB.Reports.Select(x => new ReportDTO { TaskId = x.Task.Id, ReportId = x.Id, TaskName = x.Task.Title, TaskLevel = x.Task.AffectedSectors.Count(), Status = x.Status.ToString() }).ToArray();
            ViewData["Reports"] = reports;
            return View();
        }

        [Authorised]
        public IHttpResponse Details(int id)
        {
            if (GetRole() != UserRole.Admin)
            {
                return MessageError("Only Admins can view Reports");
            }
            ReportDetailsDTO foundReportDTO = DB.Reports.Where(x => x.Id == id).Select(x => new ReportDetailsDTO
            {
                ReportId = x.Id,
                TaskName=x.Task.Title,
                DueDate = x.Task.DueDate == null ? "" : x.Task.DueDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ReportedOn = x.ReportedOn.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ReporterName = x.Reporter.Username,
                TaskLevel = x.Task.AffectedSectors.Count(),
                Status = x.Status.ToString(),
                Participants = string.Join(", ", x.Task.TaskParticipants.Select(tp => tp.Participant.Name).ToArray()),
                AffectedSectors = string.Join(", ", x.Task.AffectedSectors.Select(ts => ts.Sector.Name).ToArray()),
                TaskDescription = x.Task.Description
            }).FirstOrDefault();
            if (foundReportDTO is null)
            {
                return MessageError("Non existing report");
            }

            ViewData["Report"] = foundReportDTO;
            return View();
        }


        [Authorised]
        public IHttpResponse Create(int id)
        {
            Task foundTask = DB.Tasks.FirstOrDefault(x => x.Id == id);
            if (foundTask is null)
            {
                return MessageError("Non existing task");
            }

            Random radnom = new Random();
            var report = new Report
            {
                Status = radnom.Next(1, 100) <= 75 ? ReportStatus.Completed : ReportStatus.Archived,
                ReportedOn=DateTime.Now,
                reporterId = CurentUser.Id,
                TaskId = id,
            };
            DB.Reports.Add(report);
            DB.SaveChanges();
            return RedirectResult("/");
        }
    }
}