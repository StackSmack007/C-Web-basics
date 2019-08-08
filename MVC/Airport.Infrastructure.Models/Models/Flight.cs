namespace Airport.Infrastructure.Models.Models
{
    using System;
    using System.Collections.Generic;
    public class Flight : BaseEntity<int>
    {
        public Flight()
        {
            FlightTickets = new HashSet<Ticket>();
            FlightSeats = new HashSet<Seat>();
            PublicFlag = false;
        }

        public string Origin { get; set; }
        public string Destination { get; set; }

        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string ImgURL { get; set; }
        public bool PublicFlag { get; set; }

        public virtual ICollection<Ticket> FlightTickets { get; set; }
        public virtual ICollection<Seat> FlightSeats { get; set; }
    }
}
//A flight has destination, origin, date, time, image (url) and public flag(boolean).