using DB_Project.Entities;
using DB_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DB_Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly DBManager dBManager;
        private static OrderModel curOrder;
        //private List<IngredientForDishModel> unusedIngredients = new();
        public OrderController()
        {
            dBManager = DBManager.Instance;
        }

        public List<OrderModel> GetAllOrders()
        {
            var model = new List<OrderModel>();
            var command = dBManager.GetCommand();
            command.CommandText = "get_orders";
            command.CommandType = CommandType.StoredProcedure;
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var order = new OrderModel();
                order.OrderId = reader.GetUInt32("order_id");
                order.User.UserId = reader.GetUInt32("customer_id");
                order.Paid = reader.GetBoolean("paid");
                model.Add(order);
            }
            reader.Close();
            return model;
        }

        public OrderModel GetOrder(uint id)
        {
            var order = new OrderModel();
            var command = dBManager.GetCommand();
            command.CommandText = "get_order_by_id";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_order", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                order.OrderId = reader.GetUInt32("order_id");
                order.User.UserId = reader.GetUInt32("customer_id");
                order.Paid = reader.GetBoolean("paid");
            }
            reader.Close();
            return order;
        }

        public OrderModel GetFullOrderInfo(uint id)
        {
            var order = GetOrder(id);
            var command = dBManager.GetCommand();
            command.CommandText = "get_full_order_info";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@id_order", id);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var dish = new OrderedDish();
                dish.Dish.DishId = reader.GetUInt32("dish_id");
                dish.Waiter.UserId = reader.GetUInt32("waiter_id");
                dish.Cook.UserId = reader.GetUInt32("chef_id");
                dish.NumberOfDishes = reader.GetUInt32("number_of_dishes");
                //dish.ReceptionTime = reader.GetString("reception_time")



                //menu.Paid = reader.GetBoolean("paid");         
            }

            reader.Close();
            return null;
        }


        // GET: OrderController
        public ActionResult Index()
        {
            return View(GetAllOrders());
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
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
