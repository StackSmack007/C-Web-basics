namespace Application.ViewModels.Orders
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal SinglePrice { get; set; }

        public decimal TotalPrice => SinglePrice * Quantity;

        public ProductDto(int cakeId, int count)
        {
            ProductId = cakeId;
            Quantity = count;
        }

        public ProductDto() { }
    }
}