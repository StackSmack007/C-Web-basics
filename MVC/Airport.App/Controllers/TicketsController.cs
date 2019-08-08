namespace Airport.App.Controllers
{
    using Airport.App.ViewModels.Tickets;
    using Airport.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Linq;
    public class TicketsController : BaseController
    {
        [Authorised]
        [HttpPost]
        public IHttpResponse Book(int ticketCount, string seatClass, int flightId)
        {
            var seat = DB.Seats.Include(x => x.Tickets).Include(x => x.Flight).FirstOrDefault(x => x.FlightId == flightId && x.Class == seatClass);
            if (seat is null)
            {
                return MessageError("Seat not found!");
            }
            int seatCappacity = seat.Cappacity - seat.Tickets.Sum(x => x.Quantity);
            if (seatCappacity < ticketCount)
            {
                return MessageError($"Not enough tickets pleace choose number less or equal to {seatCappacity}");
            }
            var ticketAlreadyBooked = seat.Tickets.FirstOrDefault(x => x.UserId == CurentUser.Id&& !x.Confirmed);
            if (ticketAlreadyBooked is null)
            {
                seat.Tickets.Add(new Ticket
                {
                    Quantity = ticketCount,
                    UserId = CurentUser.Id
                });
            }
            else
            {
                ticketAlreadyBooked.Quantity += ticketCount;
            }
            DB.SaveChanges();
            return RedirectResult($"/Flights/Details?id={flightId}");
        }

        [Authorised]
        public IHttpResponse MyTickets()
        {
            var locations = DB.Flights.Where(x => x.FlightTickets.Any(t => t.UserId == CurentUser.Id && t.Confirmed))
                                       .Select(x => new MyFlightOutDto
                                       {
                                           Destination = x.Destination,
                                           ImgURL = x.ImgURL,
                                           Origin = x.Origin,
                                           Date = x.Date,
                                           Tickets = x.FlightTickets.Where(t => t.UserId == CurentUser.Id && t.Confirmed)
                                                                    .Select(t => new MyTicketDTO
                                                                    {
                                                                        Quantity = t.Quantity,
                                                                        Class = t.Seat.Class,
                                                                        Price = t.Seat.Price
                                                                    }).ToArray()
                                       }).ToArray();

            ViewData["Locations"] = locations;
            return View();
        }

        [Authorised]
        public IHttpResponse Cart()
        {
            var orders = DB.Tickets.Where(x => x.UserId == CurentUser.Id && !x.Confirmed).Select(x => new ticketOrderOutDto
            {
                Id = x.Id,
                ImgURL = x.Seat.Flight.ImgURL,
                Origin = x.Seat.Flight.Origin,
                Class = x.ClassId,
                Destination = x.Seat.Flight.Destination,
                Date = x.Seat.Flight.Date,
                TicketsCount = x.Quantity,
                Price = x.Seat.Price
            }).ToArray();
            var totalWorth = orders.Sum(x => x.TotalWorth).ToString("F2");
            ViewData["TotalPrice"] = totalWorth;
            ViewData["Orders"] = orders;
            return View();
        }

        [Authorised]
        public IHttpResponse Remove(int id)
        {
            Ticket foundTicket = DB.Tickets.FirstOrDefault(x => x.Id == id);
            if (foundTicket is null)
            {
                return MessageError("Ticket not found");
            }
            DB.Tickets.Remove(foundTicket);
            DB.SaveChanges();
            return RedirectResult("/Tickets/Cart");
        }


        [Authorised]
        public IHttpResponse CheckOut()
        {
            Ticket[] unVerifiedTickets = DB.Tickets.Where(x => x.UserId == CurentUser.Id && !x.Confirmed).ToArray();

            for (int i = 0; i < unVerifiedTickets.Length; i++)
            {
                unVerifiedTickets[i].Confirmed = true;
            }
            DB.SaveChanges();
            return RedirectResult("/Home/Index");
        }


    }
}