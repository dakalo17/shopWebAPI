namespace shopWebAPI.Models
{
	public class OrderProduct
	{
		public int Id { get; set; }
		public int Order_id { get; set; }
		public int Product_id { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
