using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace DB_Project.Models
{
    public class DishModel
    {
        public uint DishId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1.0, (double)decimal.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public bool Approved { get; set; }
        public List<IngredientForDishModel> Ingredients { get; set; } = new();
    }
}
