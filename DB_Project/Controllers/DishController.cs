using DB_Project.Entities;
using DB_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace DB_Project.Controllers
{
    public class DishController : Controller
    {
        private readonly DBManager dBManager;
        private static DishModel curDish;
        private List<IngredientForDishModel> unusedIngredients = new();
        public DishController() 
        {
            dBManager = DBManager.Instance;
        }
        // GET: DishController
        public ActionResult Index()
        {
            return View(GetDishes().Values.ToList());
        }
        //public ActionResult Index(List<DishModel> dishes)
        //{
        //    return View(dishes);
        //}

        public Dictionary<uint, DishModel> GetDishes()
        {
            var model = new Dictionary<uint, DishModel>();
            var command = dBManager.GetCommand();
            command.CommandText = "get_dishes";
            command.CommandType = CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var dish = new DishModel();
                dish.DishId = reader.GetUInt32("dish_id");
                dish.Name = reader.GetString("name");
                dish.Description = reader.GetString("description");
                dish.Price = reader.GetDecimal("price");
                dish.Approved = reader.GetBoolean("approved");
                model.Add(dish.DishId, dish);
            }
            reader.Close();
            return model;
        }

        public Dictionary<uint, DishModel> GetDishesWithRecipes()
        {
            var model = GetDishes();
            var command = dBManager.GetCommand();
            command.CommandText = "get_dishes_recipes";
            command.CommandType = CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var dishId = reader.GetUInt32("dish_id");
                var ingredient = new IngredientForDishModel
                {
                    IngredientId = reader.GetUInt32("ingredient_id"),
                    Name = reader.GetString("ingredient name"),
                    Gram = reader.GetDouble("gram")
                };
                model[dishId].Ingredients.Add(ingredient);
            }
            reader.Close();
            return model;
        }


        public DishModel GetDish(uint id)
        {
            var dish = new DishModel();
            var command = dBManager.GetCommand();
            command.CommandText = "get_dish_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@id_dish", MySqlDbType.UInt32).Value = id;
            command.Parameters["@id_dish"].Direction = ParameterDirection.Input;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                dish.DishId = reader.GetUInt32("dish_id");
                dish.Name = reader.GetString("name");
                dish.Description = reader.GetString("description");
                dish.Price = reader.GetDecimal("price");
                dish.Approved = reader.GetBoolean("approved");
            }
            reader.Close();
            return dish;
        }

        public DishModel GetDishRecipe(uint id)
        {
            var dish = GetDish(id);
            var command = dBManager.GetCommand();
            command.CommandText = "get_ingredients_recipe";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@id_dish", MySqlDbType.UInt32).Value = id;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var ingredient = new IngredientForDishModel();
                ingredient.IngredientId = reader.GetUInt32("ingredient_id");
                ingredient.Name = reader.GetString("ingredient name");
                ingredient.Description = reader.GetString("description");
                ingredient.PricePerKilogram = reader.GetDecimal("price_per_kilogram");
                if (!reader.IsDBNull("gram"))
                {
                    ingredient.Gram = reader.GetDouble("gram");
                    dish.Ingredients.Add(ingredient);
                    continue;
                }
                unusedIngredients.Add(ingredient);         
            }
            TempData["UnusedIngredients"] = JsonConvert.SerializeObject(unusedIngredients);
            reader.Close();
            return dish;
        }

        // GET: DishController/Details/5
        public ActionResult Details(int id)
        {
            return View(GetDishRecipe((uint)id));
        }

        public ActionResult DetailsIngredient(int id)
        {
            var ingredient = new IngredientForDishModel();
            var command = dBManager.GetCommand();
            command.CommandText = "get_ingredient_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_ingredient", (uint)id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ingredient.IngredientId = reader.GetUInt32("ingredient_id");
                ingredient.Name = reader.GetString("name");
                ingredient.Description = reader.GetString("description");
                ingredient.PricePerKilogram = reader.GetDecimal("price_per_kilogram");
            }
            reader.Close();
            return View(ingredient);
        }

        // GET: DishController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DishController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm]DishModel dish)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "create_dish_without_ingredients";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@d_name", dish.Name);
            command.Parameters.AddWithValue("@d_description", dish.Description);
            command.Parameters.AddWithValue("@d_price", dish.Price);
            command.Parameters.AddWithValue("@d_approved", dish.Approved);
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult CreateIngredient()
        {
            return View();
        }

        // POST: DishController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIngredient([FromForm]IngredientForDishModel ingredient)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "create_ingredient";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@i_name", ingredient.Name);
            command.Parameters.AddWithValue("@i_description", ingredient.Description);
            command.Parameters.AddWithValue("@i_price_per_kilogram", ingredient.PricePerKilogram);
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult AddIngredient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIngredient([FromForm] IngredientForDishModel ingredient)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "add_ingredient_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_ingredient", ingredient.IngredientId);
            command.Parameters.AddWithValue("@id_dish", curDish.DishId);
            command.Parameters.AddWithValue("@ingredient_gram", ingredient.Gram);
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Edit));
            }
            catch
            {
                return View();
            }
        }

        // GET: DishController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                curDish = GetDishRecipe(curDish.DishId);
            }
            else
            {
                curDish = GetDishRecipe((uint)id);
            }
            return View(curDish);
        }

        // POST: DishController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, /*[FromForm]DishModel dish*/IFormCollection collection)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "update_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_dish", (uint)id);
            var val = collection["Name"].ToString();
            command.Parameters.AddWithValue("@d_name", val);
            command.Parameters.AddWithValue("@d_description", collection["Description"].ToString());
            command.Parameters.AddWithValue("@d_price", decimal.Parse(collection["Price"].ToString()));
            command.Parameters.AddWithValue("@d_approved", bool.Parse(collection["Approved"][0].ToString()));
            //var zip = collection["item.Name"].Zip(collection["item.Gram"]);

            //command.Parameters.AddWithValue("@d_name", dish.Name.ToString());
            //command.Parameters.AddWithValue("@d_description", dish.Description.ToString());
            //var d = decimal.Parse(dish.Price.ToString(), CultureInfo.InvariantCulture);
            //command.Parameters.AddWithValue("@d_price", d);
            //command.Parameters.AddWithValue("@d_approved", bool.Parse(dish.Approved.ToString()));
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditIngredient(int id)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "get_ingredient_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_ingredient", (uint)id);
            command.Parameters.AddWithValue("@id_dish", curDish.DishId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                TempData["OldRecipeId"] = JsonConvert.SerializeObject(reader.GetUInt32("ingredient_dish_id"));
            }
            reader.Close();
            return View(curDish.Ingredients.FirstOrDefault(i => i.IngredientId == (uint)id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIngredient(int id, [FromForm]IngredientForDishModel ingredient) 
        {
            var command = dBManager.GetCommand();
            command.CommandText = "update_ingredient_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_ingredient_dish", JsonConvert.DeserializeObject<uint>(TempData["OldRecipeId"] as string));
            command.Parameters.AddWithValue("@id_ingredient",ingredient.IngredientId);
            command.Parameters.AddWithValue("@id_dish", curDish.DishId);
            command.Parameters.AddWithValue("@ingredient_gram", ingredient.Gram);
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Edit));
            }
            catch
            {
                return View();
            }
        }


        public ActionResult DeleteIngredient(int id)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "delete_ingredient_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_ingredient", (uint)id);
            command.Parameters.AddWithValue("@id_dish", curDish.DishId);
            command.ExecuteNonQuery();

            return RedirectToAction(nameof(Edit));
        }


        // GET: DishController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(curDish);
        }

        // POST: DishController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "delete_dish";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_dish", (uint)id);
            command.ExecuteNonQuery();
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
