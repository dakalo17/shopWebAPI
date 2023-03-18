using System.Globalization;
using Npgsql;
using shopWebAPI.Models;

namespace shopWebAPI.Data
{
	public class CartSqlConnection : BaseSqlConnection
	{
		public CartSqlConnection(string? connectionString) : base(connectionString)
		{
		}

		public async Task<List<Cart?>?> Select(int userId,bool active=true)
		{
			sql = "SELECT * FROM \"order\" " +
				"WHERE fk_user_id=@userId and status=1";


			List<Cart?>? carts = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@userId", userId);

				using var reader = await cmd.ExecuteReaderAsync();
				carts = new List<Cart?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						carts.Add ( new Cart
						{
							Id = Convert.ToInt32(reader["id"]),
							Fk_User_Id = Convert.ToInt32(reader["fk_user_id"]),
							Order_Date = Convert.ToDateTime(reader["order_date"]),
							Total_Cost = Convert.ToDecimal(reader["total_cost"]),
							Status = Convert.ToInt32(reader["status"]),
						});
					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}
			finally
			{
				await _connection.CloseAsync();
			}
			return carts;

		}

		public Cart? SelectNonAsync(int userId)
		{
			sql = "SELECT * FROM \"order\" " +
				"WHERE fk_user_id=@userId";


			Cart? order = null;
			try
			{
				_connection.Open();

				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@userId", userId);

				using var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					if (reader.HasRows)
					{
						order = new Cart
						{
							Id = Convert.ToInt32(reader["id"]),
							Fk_User_Id = Convert.ToInt32(reader["fk_user_id"]),
							Order_Date = Convert.ToDateTime(reader["order_date"]),
							Total_Cost = Convert.ToDecimal(reader["total_cost"]),
							Status = Convert.ToInt32(reader["status"]),
						};
					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}
			finally
			{
				_connection.Close();
			}
			return order;

		}

		public async Task<Cart?> SelectAsync(int userId)
		{
			sql = "SELECT * FROM \"order\" " +
				"WHERE fk_user_id=@userId";


			Cart? order = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@userId", userId);

				using var reader = await cmd.ExecuteReaderAsync();

				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						order = new Cart
						{
							Id = Convert.ToInt32(reader["id"]),
							Fk_User_Id = Convert.ToInt32(reader["fk_user_id"]),
							Order_Date = Convert.ToDateTime(reader["order_date"]),
							Total_Cost = Convert.ToDecimal(reader["total_cost"]),
							Status = Convert.ToInt32(reader["status"]),
						};
					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}
			finally
			{
				await _connection.CloseAsync();
			}
			return order;

		}
		public async Task<List<Cart?>?> Select()
		{
			sql = "SELECT * FROM \"order\"";

			List<Cart?>? orders = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);
			
				using var reader  = await cmd.ExecuteReaderAsync();
				orders = new List<Cart?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						orders.Add(new Cart {
							Id = Convert.ToInt32(reader["id"]),
							Fk_User_Id = Convert.ToInt32(reader["fk_user_id"]),
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
			finally
			{
				await _connection.CloseAsync();
			}
			return orders;
		}

		public async Task<Cart?> Select(int userId,int cartId)
		{
			sql = "SELECT * FROM \"order\" " +
				"WHERE fk_user_id=@userId and fk_order_id=@cartId ";


			Cart? order = null;
			try
			{
				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@userId", userId);
				cmd.Parameters.AddWithValue("@cartId", cartId);

				using var reader = await cmd.ExecuteReaderAsync();

				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						order = new Cart
						{
							Id = Convert.ToInt32(reader["id"]),
							Fk_User_Id = Convert.ToInt32(reader["fk_user_id"]),
							Order_Date = Convert.ToDateTime(reader["order_date"]),
							Total_Cost = Convert.ToDecimal(reader["total_cost"]),
							Status = Convert.ToInt32(reader["status"]),
						};
					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}
			finally
			{
				await _connection.CloseAsync();
			}
			return order;

		}

		public async Task<int> Insert(Cart order)
		{
			sql = "INSERT into \"order\"(fk_user_id, order_date, total_cost) " +
				"Values(@user_id, @order_date, @total_cost)";

			var affectedRows = 0;
			try
			{
				await _connection.OpenAsync();

				await using var cmd = new NpgsqlCommand(sql, _connection);
				
				cmd.Parameters.AddWithValue("@user_id",order.Fk_User_Id);
				cmd.Parameters.AddWithValue("@order_date",DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
				cmd.Parameters.AddWithValue("@total_cost",order.Total_Cost);
				


				affectedRows = await cmd.ExecuteNonQueryAsync();
			}
			catch(Exception ex) {
				ex.GetBaseException();		
			}
			finally
			{
				await _connection.CloseAsync();
			}
			return affectedRows;
		}
	}
}
