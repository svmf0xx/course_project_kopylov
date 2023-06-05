using System.ComponentModel.DataAnnotations;

namespace JewStore.Models
{
	public class FeedbackModel
	{
		[Key]
		public int FbId { get; set; }
		public string ClientName { get; set; }
		public string FbScore { get; set; }
		public string FbText { get; set; }
		public string FbMasterName { get; set;}
		public int OrderId { get; set; }
	}
}
