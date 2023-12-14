using Microsoft.Build.Framework;

namespace DB_Project.Models
{
    public class IngredientForDishModel
    {
        public uint IngredientId { get; set; }
        public string Name { get;set; }
        public string Description { get; set; }
        public decimal PricePerKilogram { get; set; }
        [Required]
        public double Gram { get; set; }

    }
}
