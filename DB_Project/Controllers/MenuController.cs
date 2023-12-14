using DB_Project.Entities;
using DB_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

namespace DB_Project.Controllers
{
    public class MenuController : Controller
    {

        private readonly DBManager dBManager;
        private static MenuModel curMenu;
        //private List<IngredientForDishModel> unusedIngredients = new();
        public MenuController()
        {
            dBManager = DBManager.Instance;
        }


        public List<MenuModel> GetMenus()
        {
            var model = new List<MenuModel>();
            var command = dBManager.GetCommand();
            command.CommandText = "get_menus";
            command.CommandType = CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var menu = new MenuModel();
                menu.MenuId = reader.GetUInt32("menu_id");
                menu.Type = reader.GetString("type");
                menu.Description = reader.GetString("description");
                model.Add(menu);
            }
            reader.Close();
            return model;
        }


        public MenuModel GetMenu(uint id)
        {
            var menu = new MenuModel();
            var command = dBManager.GetCommand();
            command.CommandText = "get_menu_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_menu", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                menu.MenuId = reader.GetUInt32("menu_id");
                menu.Type = reader.GetString("type");
                menu.Description = reader.GetString("description");
            }
            reader.Close();
            return menu;
        }
        public MenuModel GetMenuDishes(uint id)
        {
            var menu = GetMenu(id);
            var command = dBManager.GetCommand();
            command.CommandText = "get_full_menu_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_menu", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dish = new DishModel();
                dish.DishId = reader.GetUInt32("dish_id");
                dish.Name = reader.GetString("name");
                dish.Description = reader.GetString("description");
                dish.Price = reader.GetDecimal("price");
                dish.Approved = reader.GetBoolean("approved");
                menu.Dishes.Add(dish);
            }
            reader.Close();
            return menu;
        }
        // GET: MenuController
        public ActionResult Index()
        {
            return View(GetMenus());
        }




        // GET: MenuController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                curMenu = GetMenuDishes(curMenu.MenuId);
            }
            else
            {
                curMenu = GetMenuDishes((uint)id);
            }
            return View(curMenu);
        }

        // POST: DishController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [FromForm]MenuModel menu)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "update_menu";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_menu", (uint)id);
            command.Parameters.AddWithValue("@m_description", menu.Description);
            command.Parameters.AddWithValue("@m_type", menu.Type);
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

        public ActionResult AddDish()
        {
            var unused = new List<DishModel>();
            var command = dBManager.GetCommand();
            command.CommandText = "get_dishes_not_in_menu_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_menu", curMenu.MenuId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dish = new DishModel();
                dish.DishId = reader.GetUInt32("dish_id");
                dish.Name = reader.GetString("name");
                dish.Description = reader.GetString("description");
                dish.Price = reader.GetDecimal("price");
                dish.Approved = reader.GetBoolean("approved");
                unused.Add(dish);
            }
            reader.Close();
            TempData["UnusedDishes"] = JsonConvert.SerializeObject(unused);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDish([FromForm]DishModel dish)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "add_dish_menu";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_dish", dish.DishId);
            command.Parameters.AddWithValue("@id_menu", curMenu.MenuId);
            command.Parameters.AddWithValue("@d_available", dish.Approved);
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

        public ActionResult RemoveDish(int id)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "delete_dish_menu";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_dish", (uint)id);
            command.Parameters.AddWithValue("@id_menu", curMenu.MenuId);

            command.ExecuteNonQuery();

            return RedirectToAction(nameof(Edit));
        }

        // GET: MenuController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(GetMenuDishes((uint)id));
        }

        // POST: MenuController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "delete_menu";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_menu", (uint)id);
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

        // GET: DishController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DishController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] MenuModel menu)
        {
            var command = dBManager.GetCommand();
            command.CommandText = "create_menu_without_dishes";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@m_type", menu.Type);
            command.Parameters.AddWithValue("@m_description", menu.Description);
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
