using Npgsql;
using shopWebAPI.Models;

namespace shopWebAPI.Data
{
	public class ProductSqlConnection : BaseSqlConnection
	{
		public ProductSqlConnection(string? connectionString) : base(connectionString)
		{
		}
		[Obsolete]
		public async Task<int> Put(Product? product) {
			return 1;
		}
		public async Task<int> Insert(Product product) {
			sql = "Insert INTO \"product\"(name,price,special_price,description,quantity,image_link) " +
				"VALUES(@name,@price,@sprice,@descr,@quantity,@imgLink)";
			
			var rowsAffected = 0;
			try
			{
				await _connection.OpenAsync();
				using (var cmd = new NpgsqlCommand(sql, _connection))
				{
					cmd.Parameters.AddWithValue("@name",product.Name?? string.Empty);
					cmd.Parameters.AddWithValue("@price",product.Price);
					cmd.Parameters.AddWithValue("@sprice",product.SpecialPrice);
					cmd.Parameters.AddWithValue("@descr",product.Description ?? string.Empty);
					cmd.Parameters.AddWithValue("@quantity", product.Quantity);
					cmd.Parameters.AddWithValue("@imgLink", product.ImageLink??string.Empty);

					rowsAffected = await cmd.ExecuteNonQueryAsync();
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
			}
			
			return rowsAffected;
		}

		public async Task<List<Product?>?> Select() {
			sql = "SELECT * FROM \"product\" ";

			using var cmd = new NpgsqlCommand(sql,_connection);

			List<Product?>? products = null;
			try
			{
				await _connection.OpenAsync();
				await using var reader = await cmd.ExecuteReaderAsync();

				products = new List<Product?>();

				while (await reader.ReadAsync()) {
					if (reader.HasRows) {
						products.Add(new Product
						{
							Id = Convert.ToInt32(reader["id"]),
							Name = reader["name"].ToString(),
							Price = Convert.ToDecimal(reader["price"]),
							SpecialPrice = Convert.ToDecimal(reader["special_price"]),
							Description = reader["description"].ToString(),
							Quantity = Convert.ToInt32(reader["quantity"]),
							ImageLink = reader["image_link"].ToString()
						});
					}
				}
				if (!reader.IsClosed)
					await reader.CloseAsync();
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

			return products;
		}

		public async Task<Product?> Select(string name) => await Select(-1,name);
		
		public async Task<Product?> Select(int id,string name="") {
			sql =	"SELECT * FROM \"product\" " +
					"WHERE id=@id OR name=@name;";

			using var cmd = new NpgsqlCommand(sql, _connection);

			Product? product = null;
			try
			{
				using var reader = await cmd.ExecuteReaderAsync();
				
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						product=new Product
						{
							Id = Convert.ToInt32(reader["id"]),
							Name = reader["name"].ToString(),
							Price = Convert.ToDecimal(reader["price"]),
							SpecialPrice = Convert.ToDecimal(reader["special_price"]),
							Description = reader["description"].ToString(),
							Quantity = Convert.ToInt32(reader["quantity"]),
							ImageLink = reader["image_link"].ToString()
						};
					}
				}
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}

			return product;
		}
		
	}
}
