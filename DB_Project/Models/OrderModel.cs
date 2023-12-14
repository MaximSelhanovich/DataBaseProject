namespace DB_Project.Models
{
    public class OrderModel
    {
        public uint OrderId { get; set; }
        public UserModel User { get; set; } = new UserModel();
        public UserModel Waiter { get; set; } = new UserModel();
        public PaymentModel Payment{ get; set; } = new PaymentModel();
        public bool Paid { get; set; }
        public List<OrderedDish> OrderedDishes { get; set; } = new();
    }
}
