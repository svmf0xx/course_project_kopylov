using Microsoft.EntityFrameworkCore;
using JewStore.Models;
namespace JewStore.Data
{
	public class AppDbContext : DbContext
	{

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
		{
			Database.EnsureCreated();
		}

		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<UserModel> Users { get; set; }
		public DbSet<FeedbackModel> Feedbacks { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OrderModel>().ToTable("Orders");
			modelBuilder.Entity<UserModel>().ToTable("Users");
			modelBuilder.Entity<FeedbackModel>().ToTable("Feedbacks");
		}

	}
}
