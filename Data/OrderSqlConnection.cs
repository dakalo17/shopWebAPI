using Npgsql;
using shopWebAPI.Models;

namespace shopWebAPI.Data
{
	public class OrderSqlConnection : BaseSqlConnection
	{
		public OrderSqlConnection(string? connectionString) : base(connectionString)
		{
		}


		public async Task<Order?> Select(int userId)
		{
			sql = "SELECT * FROM \"order\" " +
				"WHERE user_id=@userId";


			Order? order = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@userId", userId);

				using var reader = await cmd.ExecuteReaderAsync();

				while (await reader.ReadAsync()) {
					if (reader.HasRows)
					{
						order = new Order
						{
							Id = Convert.ToInt32(reader["id"]),
							User_Id = Convert.ToInt32(reader["user_id"]),
							Order_Date = Convert.ToDateTime(reader["order_date"]),
							Total_Cost = Convert.ToDecimal(reader["total_cost"])
						};
					}
				}
			}
			catch(Exception ex) 
			{
				ex.GetBaseException();
				return null;
			}
			return order;

		}
		public async Task<List<Order?>?> Select()
		{
			sql = "SELECT * FROM \"order\"";

			List<Order?>? orders = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);
			
				using var reader  = await cmd.ExecuteReaderAsync();
				orders = new List<Order?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						orders.Add(new Order {
							Id = Convert.ToInt32(reader["id"]),
							User_Id = Convert.ToInt32(reader["user_id"]),
							Order_Date =Convert.ToDateTime(reader["order_date"]),
							Total_Cost  = Convert.ToDecimal(reader["total_cost"])
						});
					}
				}
			}
			catch(Exception ex) 
			{
				ex.GetBaseException();
				return null;
			}
			return orders;
		}
		public async Task<int> Insert(Order order)
		{
			sql = "INSERT into \"order\"(user_id, order_date, total_cost) " +
				"Values(@user_id, @order_date, @total_cost)";

			var affectedRows = 0;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);
				
				cmd.Parameters.AddWithValue("@user_id");
				cmd.Parameters.AddWithValue("@order_date");
				cmd.Parameters.AddWithValue("@total_cost");
				


				affectedRows = await cmd.ExecuteNonQueryAsync();
			}
			catch(Exception ex) {
				ex.GetBaseException();		
			}
			return affectedRows;
		}
	}
}
