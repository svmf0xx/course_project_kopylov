using JewStore.Data;
using JewStore.Interfaces;
using JewStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;

namespace JewStore.Controllers
{

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IResourceService _resourceService;
		private readonly AppDbContext _dbContext;
		public HomeController(AppDbContext dbContext, ILogger<HomeController> logger, IResourceService resourceService)
		{
			_dbContext = dbContext;
			_resourceService = resourceService;
			_logger = logger;
		}

		public async Task<IActionResult> AddOrder(OrderModel order)
		{
			await _resourceService.AddOrderAsync(new OrderModel { Price = Math.Round(CalculatePrice(order), 2), ClientName = order.ClientName, ClientLogin = order.ClientLogin, Type = order.Type, Material = order.Material, Gem = order.Gem, Comment = order.Comment });
			return CheckLoginAfterOrder(new UserModel { Login = order.ClientLogin });
		}

		public async Task<IActionResult> DeleteOrder(OrderModel order)
		{
			var name = order.ClientLogin;
			await _resourceService.DeleteOrderAsync(order);
			return CheckLoginAfterOrder(new UserModel { Login = name });
		}
		[HttpGet]
		public IActionResult ClientPage(UserModel user)
		{
			return View(user);
		}
		public IActionResult _ClientOrders(UserModel user)
		{
			ViewBag.User = user.UserName;
			var res = _resourceService.GetAllOrdersForClient(user).Result;
			return PartialView("_ClientOrders", res);
		}

		public IActionResult _MakeOrder(OrderModel order)
		{

			return PartialView("_MakeOrder", order);
		}
		public IActionResult _MakeFeedback(FeedbackModel fb)
		{
			return PartialView("_MakeFeedback", fb);
		}
		public async Task<IActionResult> AddFeedback(FeedbackModel fb)
		{
			string client = "";

			foreach (var user in _resourceService.GetAllUsers().Result)
			{
				if (user.UserName == fb.ClientName)
				{
					client = user.Login;
				}
			}
			await _resourceService.AddFeedbackAsync(fb);
			return CheckLoginAfterOrder(new UserModel { Login = client });
		}
		public double CalculatePrice(OrderModel order)
		{
			double price = 0;

			switch (order.Type)
			{
				default: break;
				case "Браслет": price += 3; break;
				case "Серьги": price += 2; break;
				case "Кольцо": price += 3; break;
				case "Подвеска": price += 4; break;
				case "Цепочка": price += 2; break;
			}

			switch (order.Material)
			{
				default: break;
				case "Золото": price *= 3.2; break;
				case "Платина": price *= 2.7; break;
				case "Иридий": price *= 1.6; break;
				case "Палладий": price *= 2.1; break;
				case "Осмий": price *= 1.9; break;
				case "Серебро": price *= 2.52; break;
			}
			switch (order.Gem)
			{
				default: break;
				case "Бриллиант": price += 15.23; break;
				case "Изумруд": price += 12.32; break;
				case "Сапфир": price += 11.43; break;
				case "Рубин": price += 14.21; break;
				case "Лазурит": price += 9.32; break;
				case "Аметист": price += 10.34; break;
				case "Нет": break;
			}

			return price;
		}







		public IActionResult AdminPage(UserModel user)
		{
			return View(user);
		}
		public async Task<IActionResult> AcceptOrder(OrderModel order)
		{
			await _resourceService.AcceptOrderAsync(order);
			return CheckLoginAfterOrder(new UserModel { Login = order.ClientLogin });
		}

		public IActionResult _AdminOrders(UserModel user)
		{
			ViewData["admin"] = user.Login;
			var res = _resourceService.GetAllOrders().Result;
			return PartialView("_AdminOrders", res);
		}

		public IActionResult _AdminMastersList(OrderModel order)
		{
			ViewData["OrderID"] = order.OrderID;
			ViewData["admin"] = order.ClientLogin;
			var res = _resourceService.GetAllUsers().Result;
			return PartialView("_AdminMastersList", res);
		}

		public IActionResult _AdminRegister(UserModel user)
		{
			ViewBag.admin = user.Login;
			return PartialView("_AdminRegister");
		}
		public IActionResult _AdminUsers(UserModel user)
		{
			var res = _resourceService.GetAllUsers().Result;
			return PartialView(res);
		}
		public async Task<IActionResult> RegisterEmployee(UserModel user)
		{
			string admin = (TempData["admin"]).ToString();

			var users = _resourceService.GetAllUsers().Result;
			foreach (var i in users)
			{
				if (user.Login == i.Login)
				{
					return RedirectToAction("RegisterPage", user);
				}
			}
			await _resourceService.AddEmployee(user);
			return CheckLoginAfterOrder(new UserModel { Login = admin });


		}








		public IActionResult MasterPage(UserModel user)
		{
			_resourceService.CalculateRating(user);
			return View(user);            //MASTERPAGE
		}

		public IActionResult _MasterOrders(UserModel user)
		{
			ViewBag.master = user.Login;
			var res = _resourceService.GetAllOrdersForMaster(user).Result;
			return PartialView("_MasterOrders", res);
		}
		public IActionResult _MasterFeedbacks(UserModel user)
		{ 
			
			var res = _resourceService.GetAllFeedbacks(user).Result;
			return PartialView("_MasterFeedbacks", res);
		}
		public async Task<IActionResult> MasterDoneOrder(OrderModel order)
		{
			var master = order.ClientLogin;
			await _resourceService.DoneOrderAsync(order);
			return CheckLoginAfterOrder(new UserModel { Login = master });

        }





        public IActionResult CheckLoginAfterOrder(UserModel user)
        {
            var db = _resourceService.GetAllUsers().Result;

            foreach (var i in db)
            {
                if (i.Login == user.Login)
                {
                    user.UserId = i.UserId;
                    user.Role = i.Role;
                    user.Rating = i.Rating;
                    user.PasswordHash = i.PasswordHash;
                    user.UserName = i.UserName;
                    switch (i.Role)
                    {
                        case "Client": return RedirectToAction("ClientPage", user);
                        case "Admin": return RedirectToAction("AdminPage", user);
                        case "Master": return RedirectToAction("MasterPage", user);
                    }
                }
            }

            return RedirectToAction("LoginPage", user);
        }
        public IActionResult CheckLogin(UserModel user)
		{
			var db = _resourceService.GetAllUsers().Result;

			foreach (var i in db)
			{
				if (i.Login == user.Login && i.PasswordHash == user.PasswordHash)
				{
					user.UserId = i.UserId;
					user.Role = i.Role;
					user.Rating=i.Rating;
                    user.UserName = i.UserName;
                    switch (i.Role)
					{
						case "Client": return RedirectToAction("ClientPage", user);
						case "Admin": return RedirectToAction("AdminPage", user);
						case "Master": return RedirectToAction("MasterPage", user);
					}
				}
			}

			return RedirectToAction("LoginPage", user);
		}

		public IActionResult LoginPage(UserModel user)		
		{
			return View(user);
		}

        public IActionResult RegisterPage(UserModel user)
        {
            return View(user);
        }
		public async Task<IActionResult> RegisterClient(UserModel user)
		{
			var users = _resourceService.GetAllUsers().Result;
			foreach (var i in users)
			{
				if(user.Login == i.Login)
				{
                    return RedirectToAction("RegisterPage", user);
                }
			}
            await _resourceService.AddUser(user);
            return RedirectToAction("LoginPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}