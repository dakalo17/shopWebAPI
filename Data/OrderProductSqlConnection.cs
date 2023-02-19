using Npgsql;
using shopWebAPI.Models;

namespace shopWebAPI.Data
{
	public class OrderProductSqlConnection : BaseSqlConnection
	{
		public OrderProductSqlConnection(string? connectionString) : base(connectionString)
		{
		}

		public async Task<List<OrderProduct?>?> Select()
		{
			sql = "SELECT * FROM \"order_product\" ";

			List<OrderProduct?>? ops = null;

			try
			{
				using var cmd = new NpgsqlCommand(sql, _connection);
				using var reader = await cmd.ExecuteReaderAsync();
				ops = new List<OrderProduct?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						ops.Add(new OrderProduct
						{
							Id = Convert.ToInt32(reader["id"]),
							Order_id = Convert.ToInt32(reader["order_id"]),
							Price = Convert.ToDecimal(reader["id"]),
							Product_id = Convert.ToInt32(reader["product_id"]),
							Quantity = Convert.ToInt32(reader["quantity"]),

						});

					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}

			return ops;
		}

		public async Task<List<OrderProduct?>?> SelectOnOrders(int orderId)
		{
			sql = "SELECT * FROM \"order_product\" " +
				"WHERE order_id = @orderId";



			List<OrderProduct?>? ops = null;

			try {
				using var cmd = new NpgsqlCommand(sql, _connection);
				cmd.Parameters.AddWithValue("@orderId", orderId);

				using var reader = await cmd.ExecuteReaderAsync();
				ops = new List<OrderProduct?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						ops.Add(new OrderProduct
						{
							Id = Convert.ToInt32(reader["id"]),
							Order_id = Convert.ToInt32(reader["order_id"]),
							Price = Convert.ToDecimal(reader["id"]),
							Product_id = Convert.ToInt32(reader["product_id"]),
							Quantity = Convert.ToInt32(reader["quantity"]),

						});

					}
				}
			} 
			catch (Exception ex) {
				ex.GetBaseException();
				return null;
			}

			return ops;
		}

		public async Task<OrderProduct?> Select(int orderProductId)
		{
			sql = "SELECT * FROM \"order_product\" " +
				"WHERE id=@opId";

			OrderProduct? op = null;

			try
			{
				using var cmd = new NpgsqlCommand(sql, _connection);
				cmd.Parameters.AddWithValue("@opId", orderProductId);

				using var reader = await cmd.ExecuteReaderAsync();
				
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						op=(new OrderProduct
						{
							Id = Convert.ToInt32(reader["id"]),
							Order_id = Convert.ToInt32(reader["order_id"]),
							Price = Convert.ToDecimal(reader["id"]),
							Product_id = Convert.ToInt32(reader["product_id"]),
							Quantity = Convert.ToInt32(reader["quantity"]),

						});

					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}

			return op;
		}

		public async Task<int> Insert(OrderProduct op)
		{
			sql = "INSERTED into \"order_product\"(order_id, product_id, quantity, price)" +
				"Values(@order_id, @product_id, @quantity, @price)";

			var affectedRows = 0;
			try {
				using var cmd = new NpgsqlCommand(sql, _connection);
				cmd.Parameters.AddWithValue("@order_id",op.Order_id);
				cmd.Parameters.AddWithValue("@product_id",op.Product_id);
				cmd.Parameters.AddWithValue("@quantity",op.Quantity);
				cmd.Parameters.AddWithValue("@price",op.Price);
				affectedRows = await cmd.ExecuteNonQueryAsync();

			} catch (Exception ex) 
			{
				ex.GetBaseException();
			}

			return affectedRows;
		}
	}
}
