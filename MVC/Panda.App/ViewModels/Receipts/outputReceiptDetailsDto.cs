namespace Panda.App.ViewModels.Receipts
{
    public class outputReceiptDetailsDto
    {
        public int Id { get; set; }
        public string Recepient { get; set; }
        public decimal PackageWeight { get; set; }
        public string PackageDescription { get; set; }
        public string DeliveryAddress { get; set; }
        public string IssuedOn { get; set; }
        public decimal Fee { get; set; }
    }
}