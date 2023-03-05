using System.Security.Cryptography;

namespace shopWebAPI.Utilities
{
	
	public class RefreshTokenGenerator
	{



		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[32];

			using (var rand = RandomNumberGenerator.Create()) {
				rand.GetBytes(randomBytes);
				return Convert.ToBase64String(randomBytes);
			}
			
		}
	}
}
