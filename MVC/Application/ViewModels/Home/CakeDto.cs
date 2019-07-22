namespace Application.ViewModels.Home
{
    public class CakeDto
    {
        public int CakeId { get; set; }
        public string CakeName { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public string Manufacturer { get; set; }

        public CakeDto() { }

        public CakeDto(string cakeName, decimal price, string imgURL, string manufacturer)
        {
            CakeName = cakeName;
            Price = price;
            ImgUrl = imgURL;
            Manufacturer = manufacturer;
        }

        public CakeDto(string cakeName, decimal price, string imgURL, string manufacturer,int cakeId) : this(cakeName, price, imgURL, manufacturer)
        {
            CakeId = cakeId;
        }
 
    }
}