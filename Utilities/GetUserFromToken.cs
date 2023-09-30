using Microsoft.AspNetCore.Mvc;


namespace shopWebAPI.Utilities
{
	public class GetUserFromToken
	{
		

		public static string? GetToken(HttpRequest Request)
		{
			var auth = Request.Headers["Authorization"];
			return auth[0]?.Split(" ")[1];

			


		}
	}
}
