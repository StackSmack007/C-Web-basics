using System;

namespace Airport.App.ViewModels.Tickets
{
    public  class ticketOrderOutDto
    {
        public int Id { get; set; }
        public string ImgURL { get; set; }
        public string Origin { get; set; }
        public string Class { get; set; }
        public string Destination { get; set; }
        public DateTime Date { get; set; }
        public int TicketsCount { get; set; }
        public decimal TotalWorth => TicketsCount * Price;
        public decimal Price { get; set; }

    }
}
