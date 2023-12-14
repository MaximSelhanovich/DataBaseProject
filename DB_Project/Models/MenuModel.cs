namespace DB_Project.Models
{
    public enum MenuType
    {
        Mornin,
        Day,
        Evening
    }
    public class MenuModel
    {
        public uint MenuId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<DishModel> Dishes { get; set; } = new();
    }
}
