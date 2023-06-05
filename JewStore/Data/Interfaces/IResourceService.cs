using JewStore.Models;

namespace JewStore.Interfaces
{
	public interface IResourceService
	{
		public Task<List<OrderModel>> GetAllOrdersForClient(UserModel user);

        public Task<List<UserModel>> GetAllUsers();
		public Task AddOrderAsync(OrderModel order);
        Task AddUser(UserModel user);
        Task DeleteOrderAsync(OrderModel order);
		Task AcceptOrderAsync(OrderModel order);
        Task<List<OrderModel>> GetAllOrders();
        Task AddEmployee(UserModel user);
		Task<List<OrderModel>> GetAllOrdersForMaster(UserModel user);
        public Task DoneOrderAsync(OrderModel order);
		Task AddFeedbackAsync(FeedbackModel fb);
        Task CalculateRating(UserModel user);
		Task<List<FeedbackModel>> GetAllFeedbacks(UserModel master);
	}
}
