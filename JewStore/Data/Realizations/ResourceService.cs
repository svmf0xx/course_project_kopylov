using JewStore.Data;
using JewStore.Interfaces;
using JewStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;

namespace JewStore.Realizations
{
	public class ResourceService : IResourceService
	{
		private readonly AppDbContext _dbContext;
		private readonly ILogger<ResourceService> _logger;

		public ResourceService(ILogger<ResourceService> logger, AppDbContext dbContext)
		{
			_dbContext = dbContext;
			_logger = logger;
		}
        public async Task<List<OrderModel>> GetAllOrders()
        {
            var orders = await _dbContext.Orders
                .ToListAsync();
            return orders;
        }
        public async Task<List<OrderModel>> GetAllOrdersForClient(UserModel user)
		{
			var orders = await _dbContext.Orders
				.Where(r => r.ClientLogin == user.Login)
				.OrderByDescending(r => r.Date)
				.ToListAsync();
			return orders;
		}

		public async Task<List<OrderModel>> GetAllOrdersForMaster(UserModel user)
		{
			var orders = await _dbContext.Orders
				.Where(r => r.MastersName == user.UserName)
				.OrderByDescending(r => r.Date)
				.ToListAsync();
			return orders;
		}

		public async Task<List<UserModel>> GetAllUsers()
		{
			var users = await _dbContext.Users.OrderBy(r=>r.Role).ToListAsync();
			return users;
		}
		public async Task<List<FeedbackModel>> GetAllFeedbacks(UserModel master)
		{
			var fb = await _dbContext.Feedbacks.Where(r => r.FbMasterName == master.UserName).ToListAsync();
			return fb;
		}
		public async Task AddOrderAsync(OrderModel order)
        {
			if(order.Comment== null)
			{
				order.Comment = "";
			}
            order.Status = "Обрабатывается";
            order.Date = DateTime.Now;
			order.MastersName = "none";
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
        }
		public async Task DeleteOrderAsync(OrderModel order)
		{
			OrderModel delord = new OrderModel() { OrderID= order.OrderID };
			_dbContext.Orders.Attach(delord);
			_dbContext.Orders.Remove(delord);
            await _dbContext.SaveChangesAsync();
        }

		public async Task AddUser(UserModel user)
		{
            user.Role = "Client";
            user.Rating = -1;
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

		public async Task AddEmployee (UserModel user)
        {
			if (user.Role == "Master")
			{
				user.Rating = 0;
			}

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AcceptOrderAsync(OrderModel order)
		{
			var orders = await _dbContext.Orders
				.ToListAsync();
			foreach(var item in orders) { 
				if (order.OrderID== item.OrderID)
				{
					item.Status = "Делается";
					item.MastersName = order.MastersName;

					await _dbContext.SaveChangesAsync();
				}
			
			}
			
		}
		public async Task DoneOrderAsync(OrderModel order)
		{
            var orders = await _dbContext.Orders
								.ToListAsync();
            foreach (var item in orders)
            {
                if (order.OrderID == item.OrderID)
                {
                    item.Status = "Готов";
                    item.MastersName = order.MastersName;

                    await _dbContext.SaveChangesAsync();
                }

            }
            await _dbContext.SaveChangesAsync();
		}

		public async Task AddFeedbackAsync(FeedbackModel fb)
		{
			_dbContext.Feedbacks.Add(fb);
			await _dbContext.SaveChangesAsync();
		}
		public Task CalculateRating(UserModel user)
		{
			
            double allscores = 0;
			double countscores = 0;			

					
					foreach(var feedback in _dbContext.Feedbacks
						.Where(r=> r.FbMasterName ==user.UserName)
						.ToListAsync()
						.Result ) 
					{
						var score = Convert.ToDouble(feedback.FbScore);
						allscores += score;
						countscores ++;					
					}
                    if (allscores != 0 || countscores != 0)
                    {
                        double finalscore = allscores / countscores;
						user.Rating = Math.Round(finalscore, 1);
                        _dbContext.Users.Update(user);
                    }
			

			return _dbContext.SaveChangesAsync();
		}
    }
}
