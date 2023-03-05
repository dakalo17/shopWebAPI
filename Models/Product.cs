using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;

namespace shopWebAPI.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public decimal Price { get; set; }
		public decimal SpecialPrice { get; set; }
		public string? Description { get; set; }
		public int Quantity { get; set; }
		public string? ImageLink { get; set; }

	}
}
