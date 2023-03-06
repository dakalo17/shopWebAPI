namespace shopWebAPI.Models.Jwt
{
	public class RefreshToken
	{
		public int? FkUserId { get; set; }
		public string RToken { get; set; } = string.Empty;
		public string? Key { get; set; }
		public DateTime ExpiringDate { get; set; }
		
	}
}
