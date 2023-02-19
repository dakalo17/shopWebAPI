namespace shopWebAPI.Models
{
	public class Order
	{
		public int Id { get; set; }
		public int Fk_User_Id { get; set; }
		public DateTime Order_Date { get; set; }
		public decimal Total_Cost { get; set; }

	}
}
