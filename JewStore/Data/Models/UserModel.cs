using System.ComponentModel.DataAnnotations;

namespace JewStore.Models
{
	public class UserModel
	{
		[Key]
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string Login { get; set; }
		public string PasswordHash { get; set; }
		public string Role { get; set; }
		public double Rating { get; set; }

	}
}
