namespace Airport.App.Controllers
{
    using Airport.App.ViewModels.Flights;
    using Airport.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Globalization;
    using System.Linq;

    public class FlightsController : BaseController
    {
        [Authorised("Admin", "/")]
        public IHttpResponse Add()
        {
            return View();
        }
        [Authorised("Admin", "/")]
        [HttpPost]
        public IHttpResponse Add(inputFlightDto flightDto)
        {
            if (DB.Flights.Any(x => x.Origin == flightDto.Origin && x.Destination == flightDto.Destination && x.Date == flightDto.Date))
            {
                return MessageError("Fligth already Exists");
            }

            Flight newFlight = new Flight
            {
                Destination = flightDto.Destination,
                Origin = flightDto.Origin,
                Date = flightDto.Date,
                ImgURL = flightDto.ImgURL,
            };
            DB.Flights.Add(newFlight);
            DB.SaveChanges();
            return RedirectResult("/");
        }

        [HttpPost]
        public IHttpResponse AddSeats(seatDTO seats, int flightId)
        {
            var flight = DB.Flights.FirstOrDefault(x => x.Id == flightId);
            if (flight is null)
            {
                return MessageError($"Unfound Flight id {flightId}");
            }
            DB.Entry(flight).Collection(s => s.FlightSeats).Load();
            var existingSeats = flight.FlightSeats.FirstOrDefault(x => x.Class == seats.Class);

            if (existingSeats != null)
            {
                existingSeats.Price = seats.Price;
                existingSeats.Cappacity += seats.SeatsAvailable;
            }
            else
            {
                flight.FlightSeats.Add(new Seat
                {
                    Class = seats.Class,
                    Price = seats.Price,
                    Cappacity = seats.SeatsAvailable
                });
            }
            DB.SaveChanges();
            return RedirectResult($"/Flights/Details?id={flightId}");
        }

        [Authorised]
        public IHttpResponse Details(int id)
        {
            outputFlightDto flight = DB.Flights.Where(x => x.Id == id).Select(x => new outputFlightDto
            {
                Id = x.Id,
                Origin = x.Origin,
                Date = x.Date.ToString("d MMMM H:mm", CultureInfo.InvariantCulture),
                Destination = x.Destination,
                ImgURL = x.ImgURL,
                IsPublic = x.PublicFlag,
                Seats = x.FlightSeats.Select(s => new seatDTO
                {
                    Class = s.Class,
                    Price = s.Price,
                    SeatsAvailable = s.Cappacity
                }).OrderBy(s => -s.Price).ToArray()
            }).FirstOrDefault();
            if (flight is null)
            {
                return MessageError("flight not found in database");
            }

            var occupiedSeats = DB.Seats.Where(x => x.FlightId == id && x.Tickets.Any())
                .Select(x => new { x.Class, BookedSeats = x.Tickets.Sum(t => t.Quantity) })
                .ToArray();
            foreach (var item in occupiedSeats)
            {
                seatDTO seat = flight.Seats.FirstOrDefault(x => x.Class == item.Class);
                seat.SeatsAvailable -= item.BookedSeats;
            }

            ViewData["Flight"] = flight;
            return View();
        }

        [HttpPost]
        [Authorised("Admin", "/")]
        public IHttpResponse Publish(int flightId)
        {
            var flight = DB.Flights.FirstOrDefault(x => x.Id == flightId);
            if (flight is null)
            {
                return MessageError($"Unfound Flight id {flightId}");
            }
            flight.PublicFlag = true;
            DB.SaveChanges();
            return RedirectResult($"/Flights/Details?id={flightId}");
        }


        [Authorised("Admin", "/")]
        public IHttpResponse RemoveSeat(int flightId, string seatClass)
        {
            var seat = DB.Seats.Include(x => x.Tickets).FirstOrDefault(x => x.FlightId == flightId && x.Class == seatClass);
            if (seat is null)
            {
                return MessageError($"Unfound Flight id {flightId} or seatClass {seatClass}");
            }

            int freeSeats = seat.Cappacity - seat.Tickets.Sum(t => t.Quantity);
            if (freeSeats <= 0)
            {
                return MessageError("Removing of booked seats not allowed!");
            }
            seat.Cappacity--;

            DB.SaveChanges();
            return RedirectResult($"/Flights/Details?id={flightId}");
        }

        public IHttpResponse Search()
        {


            return null;
        }

        [Authorised("Admin", "/")]
        public IHttpResponse Edit(int flightId)
        {
            outputFlightDto flight = DB.Flights.Where(x => x.Id == flightId)
                                       .Select(x => new outputFlightDto
                                       {
                                       Id=flightId,
                                       Destination = x.Destination,
                                       Origin = x.Origin,
                                       Date = x.Date.ToString(),
                                       ImgURL = x.ImgURL,
                                       IsPublic = x.PublicFlag,
                                       }).FirstOrDefault();
            if (flight is null)
            {
                return MessageError("Flight not found");
            }

            ViewData["Flight"] = flight;           
            return View();
        }

        [HttpPost]
        [Authorised("Admin", "/")]
        public IHttpResponse Edit(inputFlightDto flightDto,int flightId)
        {
            var flight = DB.Flights.FirstOrDefault(x => x.Id == flightId);
            if (flight is null)
            {
                return MessageError("Flight not found");
            }
            flight.Origin = flightDto.Origin;
            flight.Destination = flightDto.Destination;
            flight.Date = flightDto.Date;
            flight.PublicFlag = flightDto.IsPublic;
            flight.ImgURL = flightDto.ImgURL;
            DB.SaveChanges();
            return RedirectResult("/");
        }

    }
}
