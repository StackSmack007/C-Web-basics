namespace Chushka.App.ViewModels.Products
{
    public class productDTO
    {
        public productDTO(string name, string price, string description, string type)
        {
            Name = name;
            Price = decimal.Parse(price);
            Description = description;
            Type = type;
        }
        public productDTO(int id, string name, string price, string description, string type):this(name,price,description,type)
        {
            Id = id;
        }


        public productDTO()
        {

        }
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    }
}