using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewStore.Models
{
	public class OrderModel
	{
		[Key]
		public int OrderID { get; set; }
        [ForeignKey("Login")]
        public string ClientLogin { get; set; }
		public string ClientName { get; set; }
		public string Type { get; set; }
		public double Price { get; set; }
		public string Material { get; set; }
		public string Comment { get; set; }	
		public string Gem { get; set; }
		public string Status { get; set; }
		public string MastersName { get; set; }
		public DateTime Date { get; set; }
	}
}
