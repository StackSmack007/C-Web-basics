using System;
using System.Collections.Generic;

namespace Airport.App.ViewModels.Tickets
{
    public class MyFlightOutDto
    {
        public MyFlightOutDto()
        {
            Tickets = new HashSet<MyTicketDTO>();
        }


        public string ImgURL { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int TicketsCount { get; set; }
        public DateTime Date { get; set; }

        public virtual ICollection<MyTicketDTO> Tickets { get; set; }




    }

    public class MyTicketDTO
    {

        public string Class { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
