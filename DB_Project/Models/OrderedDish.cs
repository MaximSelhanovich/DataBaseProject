namespace DB_Project.Models
{
    public class OrderedDish
    {
        public DishModel Dish { get; set; } = new();
        public uint NumberOfDishes { get; set; }
        public UserModel Cook { get; set; } = new ();
        public UserModel Waiter { get; set; } = new();
        public DateTime ReceptionTime { get; set; }
        public DateTime CookedTime { get; set;  }
    }
}
