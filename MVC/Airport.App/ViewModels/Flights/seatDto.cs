namespace Airport.App.ViewModels.Flights
{
    public class seatDTO
    {
        public seatDTO()
        { }
        public seatDTO(decimal price, string type, int count)
        {
            Price = price;
            Class = type;
            SeatsAvailable = count;
        }
        public decimal Price { get; set; }
        public string Class { get; set; }
        public int SeatsAvailable { get; set; }
    }
}
