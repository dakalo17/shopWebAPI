using System.Numerics;
using Npgsql;
using shopWebAPI.Models;
using static System.String;

namespace shopWebAPI.Data;

public class UserSqlConnection : BaseSqlConnection
{
    public UserSqlConnection(string? connectionString) : base(connectionString)
    {
        
    }

    public async Task<User?> Select(User? userIn)
    {
        sql = "select * from \"user\" " +
              "where email=@email and password=@password;";

     
        User? user = null;
        try
        {
            await _connection.OpenAsync();

			using var cmd = new NpgsqlCommand(sql, _connection);


			cmd.Parameters.AddWithValue("@email", userIn.Email ?? string.Empty);
			cmd.Parameters.AddWithValue("@password", userIn.Password ?? string.Empty);

			using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(reader["id"]),
						FirstName = reader["firstName"].ToString(),
						LastName = reader["lastname"].ToString(),
						Email = reader["email"].ToString(),
                        Password = reader["password"].ToString()
                    };


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

        return user;

    }
    public async Task<User?> Select(int id)
    {
        sql = "select * from \"user\" " +
              "where id = @id";   

        User? user = null;        
        try
        {
            await _connection.OpenAsync();
			using var cmd = new NpgsqlCommand(sql, _connection);
			cmd.Parameters.AddWithValue("@id", id);
			using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.HasRows)
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(reader["id"]),
						FirstName = reader["firstName"].ToString(),
                        LastName = reader["lastname"].ToString(),
						Email = reader["email"].ToString(),
                        Password = reader["password"].ToString()
                    };
                }
            }
        }
        catch (Exception ex)
        {
            ex.GetBaseException();
            return null;
        }

        return user;

    }

    public async Task<int> Insert(Register? register)
    {
        sql = "insert into \"user\"(firstname, lastname, email, password) " +
			  "values(@firstname,@lastname,@email,@password)";
        
        var rowsAffected = 0;
        try
        {
            await _connection.OpenAsync();
			
            using var cmd = new NpgsqlCommand(sql, _connection);
			cmd.Parameters.AddWithValue("@firstname", register?.FirstName??Empty);
			cmd.Parameters.AddWithValue("@lastname", register?.LastName??Empty);
			cmd.Parameters.AddWithValue("@email", register?.Email??Empty);
			cmd.Parameters.AddWithValue("@password", register?.Password ?? Empty);

			rowsAffected = await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            ex.GetBaseException();
        }
        

        return rowsAffected;
    }
}