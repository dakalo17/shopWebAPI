namespace shopWebAPI.Models.Jwt
{
	public class UserRefreshToken
	{
		public string Username { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
