using Npgsql;
using shopWebAPI.Models;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace shopWebAPI.Data
{
	public class CartItemSqlConnection : BaseSqlConnection
	{
		private readonly string _connectionString;

        public CartItemSqlConnection(string? connectionString) : base(connectionString)
		{
			_connectionString = connectionString;

        }

		public async Task<List<CartItem?>?> Select(int cartId, double x)
		{
			sql = "SELECT * FROM \"order_product\" " +
				"WHERE status=1 and fk_order_id=@cartId";

			List<CartItem?>? ops = null;
			
			try
			{
				using var cmd = new NpgsqlCommand(sql, _connection);

				cmd.Parameters.AddWithValue("@cartId", cartId);


				await using var reader = await cmd.ExecuteReaderAsync();
				ops = new List<CartItem?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						ops.Add(new CartItem
						{
							//Id = Convert.ToInt32(reader["id"]),
							Fk_Order_Id = Convert.ToInt32(reader["fk_order_id"]),
							Price = Convert.ToDecimal(reader["price"]),
							Fk_Product_Id = Convert.ToInt32(reader["fk_product_id"]),
							Quantity = Convert.ToInt32(reader["quantity"]),
							Status = Convert.ToInt32(reader["status"])

						});
                        
                    }
				}
                if (!reader.IsClosed)
                    await reader.CloseAsync();
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

			return ops;
		}

		public async Task<List<CartItem?>?> Select()
		{
			sql = "SELECT * FROM \"order_product\" ";

			List<CartItem?>? ops = null;

			try
			{
				using var cmd = new NpgsqlCommand(sql, _connection);
				await using var reader = await cmd.ExecuteReaderAsync();
				ops = new List<CartItem?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						ops.Add(new CartItem
						{
							//Id = Convert.ToInt32(reader["id"]),
							Fk_Order_Id = Convert.ToInt32(reader["fk_order_id"]),
							Price = Convert.ToDecimal(reader["price"]),
							Fk_Product_Id = Convert.ToInt32(reader["fk_product_id"]),
							Quantity = Convert.ToInt32(reader["quantity"]),
							Status = Convert.ToInt32(reader["status"])

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

			return ops;
		}

		public async Task<List<Product?>?> SelectOnOrder(int orderId)
		{

            //sql = "SELECT * FROM \"order_product\" " +
            //	"WHERE order_id = @orderId";

            //	\"order_product\".id as order_product_id,   " +
            sql =	@"
					select 
					p.name, 
					p.price,
					p.image_link, 
                    p.description, 
					p.special_price, 
				
					""order_product"".quantity 
					from ""order_product"" 
					inner join ""product"" p on 
					""order_product"".fk_product_id = p.id 
                    where  ""order_product"".status= @status and ""order_product"".fk_order_id = @orderId 
                    order by ""order_product"".fk_order_id";


			List<Product?>? ops = null;

			try {
				await _connection.OpenAsync();
				using var cmd = new NpgsqlCommand(sql, _connection);
				cmd.Parameters.AddWithValue("@orderId", orderId);
				cmd.Parameters.AddWithValue("@status", 1);
				

				await using var reader = await cmd.ExecuteReaderAsync();
				ops = new List<Product?>();
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{
						ops.Add(new Product
						{
							//Cart_product_id = Convert.ToInt32(reader["order_product_id"]),
							Price = Convert.ToDecimal(reader["price"]),
							
							Quantity = Convert.ToInt32(reader["quantity"]),
							Name = reader["name"].ToString(),
							
							ImageLink = reader["image_link"].ToString(),
							SpecialPrice =Convert.ToDecimal(reader["special_price"].ToString()),
							Description = reader["description"].ToString(),
							
						});

					}
				}
			} 
			catch (Exception ex) {
				ex.GetBaseException();
				return null;
			}
			finally
			{
				await _connection.CloseAsync();
			}

			return ops;
		}

		public async Task<CartItem?> SelectCart(int orderProductId)
		{
			//sql = "SELECT * FROM \"order_product\" " +
				//"WHERE id=@opId";
				///TODO
			sql =	"select " +
					"p.name, " +
					"p.price, " +
					"p.image_link, " +
					"p.special_price, " +
					"\"order_product\".id,   " +
					"\"order_product\".quantity " +
					"from \"order_product\" " +
					"inner join \"product\" p on " +
					"\"order_product\".fk_product_id = p.id " +
					"where  \"order_product\".status= @status and \"order_product\".id = @orderProductId " +
					"order by \"order_product\".id";



			CartItem? op = null;
			CartProduct? cartProduct = null;
			try
			{
				using var cmd = new NpgsqlCommand(sql, _connection);
				cmd.Parameters.AddWithValue("@orderProductId", orderProductId);
				cmd.Parameters.AddWithValue("@status", 1);

                await using var reader = await cmd.ExecuteReaderAsync();
				
				while (await reader.ReadAsync())
				{
					if (reader.HasRows)
					{ cartProduct = new()
					{
						Name = reader["name"].ToString(),
						Price = Convert.ToDecimal(reader["price"].ToString()),
						Image_link = reader["image_link"].ToString(),
						Quantity = Convert.ToInt32(reader["quantity"].ToString()),
						Special_price = Convert.ToDouble(reader["special_price"].ToString()),
						Cart_product_id = Convert.ToInt32(reader["order_product_id"].ToString())
						};
						/*	op=(new CartItem
							{
								Id = Convert.ToInt32(reader["id"]),
								Fk_Order_Id = Convert.ToInt32(reader["fk_order_id"]),
								Price = Convert.ToDecimal(reader["price"]),
								Fk_Product_Id = Convert.ToInt32(reader["fk_product_id"]),
								Quantity = Convert.ToInt32(reader["quantity"]),
								Status = Convert.ToInt32(reader["status"])

							});
						*/
						
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

			return op;
		}

		public async Task<Product?> Insert(CartItem op,int userId)
		{
			

			var affectedRows = 0;
			Product? product = null;
			NpgsqlTransaction? transaction = null;
			try
			{
				await _connection.OpenAsync();
				transaction = await _connection.BeginTransactionAsync();

				Cart? order = null;
				const string selectCartQuery = "SELECT * FROM \"order\" " +
				"WHERE fk_user_id=@userId";
				using (var cmd = new NpgsqlCommand(selectCartQuery, _connection))
				{

					cmd.Transaction = transaction;
					cmd.Parameters.AddWithValue("@userId", userId);

                    await using var reader = await cmd.ExecuteReaderAsync();


					if (await reader.ReadAsync() && reader.HasRows)
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



				if (order == null)
				{


					//TODO- separate to insert and select
					//create cart id/entry
					const string createCart =
						"INSERT into \"order\"(fk_user_id, order_date, total_cost,status) " +
						"Values(@user_id, @order_date, @total_cost,@status) ;" +
						"";
					await using var cmd = new NpgsqlCommand(createCart, _connection);

					cmd.Transaction = transaction;
					cmd.Parameters.AddWithValue("@user_id", userId);
					cmd.Parameters.AddWithValue("@order_date", DateTime.UtcNow);
					cmd.Parameters.AddWithValue("@total_cost", 0);
					cmd.Parameters.AddWithValue("@status", 1);

					affectedRows = await cmd.ExecuteNonQueryAsync();

					if (affectedRows != 1) throw new Exception();

					//get the inserted value

					const string selectInsertedCart = @"
						select id from ""order""
						where fk_user_id = @userId;
					";

					await using var cmdget = new NpgsqlCommand(selectInsertedCart, _connection);
					cmdget.Transaction = transaction;
					cmdget.Parameters.AddWithValue("@userId", userId);

                    await using var reader = await cmdget.ExecuteReaderAsync();

					int orderIdFromCreate = 0;

					if (await reader.ReadAsync() && reader.HasRows)
					{
						orderIdFromCreate = Convert.ToInt32(reader["id"].ToString());
					}
					if (!reader.IsClosed)
						await reader.CloseAsync();
                    //if not created the return null
                    if (orderIdFromCreate <= 0) return null;




					const string insertToCartQuery = "INSERT into \"order_product\"(fk_order_id, fk_product_id, quantity, price,status)" +
													"Values(@order_id, @product_id, @quantity, @price,@status)";

                    await using var cmd1 = new NpgsqlCommand(insertToCartQuery, _connection);
					cmd1.Transaction = transaction;

					cmd1.Parameters.AddWithValue("@order_id", orderIdFromCreate);
					cmd1.Parameters.AddWithValue("@product_id", op.Fk_Product_Id);
					cmd1.Parameters.AddWithValue("@quantity", op.Quantity);
					cmd1.Parameters.AddWithValue("@price", op.Price);
					cmd1.Parameters.AddWithValue("@status", 1);

					affectedRows = await cmd1.ExecuteNonQueryAsync();


				}
				else if (op!=null )
				{
					
					int getorderId = -1;
                    if (op.Fk_Order_Id < 0)
                    {

                        //get the cart by user Id
                        
                        string getCartOrderIdByUserId = @"
						select ""order"".id from ""order""
						where ""order"".fk_user_id =@userId;";


                        await using var cmd1 = new NpgsqlCommand(getCartOrderIdByUserId, _connection, transaction);
                        
                        cmd1.Parameters.AddWithValue("@userId",userId);

                        await using var readerOrder = await cmd1.ExecuteReaderAsync();

                        if (await readerOrder.ReadAsync() && readerOrder.HasRows)
                        {
                            getorderId = Convert.ToInt32(readerOrder["id"].ToString());
							
                            
                        }
                        if (!readerOrder.IsClosed)
                            await readerOrder.CloseAsync();
                    }


                    const string updateProductCart = @"
					insert into ""order_product"" (fk_order_id, fk_product_id, quantity, price,status)
					values (@orderId, @productId, @quantity, @price,@status)
					on conflict(fk_order_id,fk_product_id)
					do update set quantity = ""order_product"".quantity+@quantity;
					";

                    op.Fk_Order_Id = int.Max(op.Fk_Order_Id, getorderId);
                    await using var cmd = new NpgsqlCommand(updateProductCart, _connection);

                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@quantity", 1);
					cmd.Parameters.AddWithValue("@productId", op.Fk_Product_Id);
					cmd.Parameters.AddWithValue("@orderId", op.Fk_Order_Id);
					cmd.Parameters.AddWithValue("@price", op.Price);
					cmd.Parameters.AddWithValue("@status", op.Status);


                    

					affectedRows = await cmd.ExecuteNonQueryAsync();
					//var reader1 = await cmd.ExecuteReaderAsync();

					
					
                    const string sql1 = "SELECT * FROM \"product\" " +
                        "WHERE id=@id;";

                    await using var cmd3 = new NpgsqlCommand(sql1, _connection);
                    cmd3.Transaction = transaction;
                    cmd3.Parameters.AddWithValue("@id", op.Fk_Product_Id);


                    await using var reader = await cmd3.ExecuteReaderAsync();

                
                    if (await reader.ReadAsync() && reader.HasRows)
                    {
                        product = new Product
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
                    if (!reader.IsClosed)
                        await reader.CloseAsync();




                }



				await transaction.CommitAsync();



			} catch (Exception ex) 
			{
				ex.GetBaseException();
				if(transaction is not null)
					await transaction.RollbackAsync();

				return null;
			}
			finally
			{
			

				await _connection.CloseAsync();
			}
			return product;
		}


		public async Task<int?> UpdateProduct(CartItem op)
		{

			var affectedRows = 0;
			try
			{
				const string updateProductCart = "update \"order_product\" " +
														"set quantity = @quantity  " +
														"where order_product.id =@cartProductId and " +
														"order_product.fk_order_id =@orderId ";

				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(updateProductCart, _connection);

				cmd.Parameters.AddWithValue("@quantity", op.Quantity);
				//cmd.Parameters.AddWithValue("@cartProductId", op.Id);
				cmd.Parameters.AddWithValue("@orderId", op.Fk_Order_Id);

				affectedRows = await cmd.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				ex.GetBaseException() ;
				return null;
			}
			finally
			{


				await _connection.CloseAsync();
			}

			return affectedRows;
		}

		public async Task<int?> UpdateProductWithCartItemId(CartItem op)
		{

			var affectedRows = 0;
			try
			{
				const string updateProductCart = "update \"order_product\" " +
														"set quantity = @quantity  " +
														"where order_product.id =@cartProductId and " +
														"order_product.fk_order_id =@orderId ";

				await _connection.OpenAsync();

				using var cmd = new NpgsqlCommand(updateProductCart, _connection);

				cmd.Parameters.AddWithValue("@quantity", op.Quantity);
				//cmd.Parameters.AddWithValue("@cartProductId", op.Id);
				cmd.Parameters.AddWithValue("@orderId", op.Fk_Order_Id);

				affectedRows = await cmd.ExecuteNonQueryAsync();
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

			return affectedRows;
		}
	}
}
