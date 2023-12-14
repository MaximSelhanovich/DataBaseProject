namespace DB_Project.Models
{
    public class PaymentModel
    {
        public uint PaymentId { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Tips { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
