namespace shopWebAPI.Models.Jwt
{
	public class JWTToken
	{
		public string Token { get; set; } =	string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
