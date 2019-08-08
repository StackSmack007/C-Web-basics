namespace Airport.Infrastructure.Models.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Seat
    {
        public Seat()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int FlightId { get; set; }
        public string Class { get; set; }
        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; }


        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public int Cappacity { get; set; }


        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}