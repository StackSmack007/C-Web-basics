namespace Airport.Infrastructure.Models.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    public class Ticket : BaseEntity<int>
    {
        public Ticket()
        {
            Confirmed = false;
        }
        [Column("CustomerId")]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int FlightId { get; set; }

        public string ClassId { get; set; }
        [ForeignKey(nameof(FlightId)+","+nameof(ClassId))]
        public virtual Seat Seat { get; set; }

        public int Quantity { get; set; }

        public bool Confirmed { get; set; }
    }
}
//  A ticket has price, class, customer and flight.FU!