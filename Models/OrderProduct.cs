namespace shopWebAPI.Models
{
	public class OrderProduct
	{
		public int Id { get; set; }
		public int Fk_Order_Id { get; set; }
		public int Fk_Product_Id { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
