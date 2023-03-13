using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using shopWebAPI.Services;

namespace shopWebAPI.Utilities
{
	
	public class RefreshTokenGenerator
	{



		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[32];

			using var rand = RandomNumberGenerator.Create();
			rand.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}

		public static int? GetUserIdFromToken(string token,IConfiguration configuration)
		{
			var param =new UserAuthenticationJWTService(configuration).
				GetTokenValidatorParams();
			
			var handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal? principal = null;

			try
			{
				principal = handler.ValidateToken(token,
					param, out var validatedToken);
			}
			catch (SecurityTokenExpiredException)
			{
				return null;

			}
			catch (SecurityTokenNoExpirationException)
			{
				return null;
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return null;
			}

			var userId = Convert.ToInt32(principal?.FindFirstValue(CustomClaimNames.Id));

			return userId;
		}
			
	}
}
