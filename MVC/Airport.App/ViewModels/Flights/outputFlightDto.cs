using System;
using System.Collections.Generic;

namespace Airport.App.ViewModels.Flights
{
    public class outputFlightDto
    {

        public outputFlightDto()
        {
            Seats = new List<seatDTO>();
        }
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Date { get; set; }
        public string ImgURL { get; set; }
        public bool IsPublic { get; set; }
        public ICollection<seatDTO> Seats { get; set; }
    }

}