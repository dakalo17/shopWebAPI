using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shopWebAPI.Models;
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



		public static User? GetUserFromToken(string token, IConfiguration configuration)
		{
			var handler = new JwtSecurityTokenHandler();

			if (string.IsNullOrEmpty(token))
				return null;

			var param = new UserAuthenticationJWTService(configuration).
				GetTokenValidatorParams();
			



			var readToken = handler.ReadJwtToken(token);



			var claims = readToken.Claims;

			var userId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == CustomClaimNames.Id)?.Value);

			var userEmail = claims.FirstOrDefault(x => x.Type == CustomClaimNames.Email)?.Value;
			var firstName = claims.FirstOrDefault(x => x.Type == CustomClaimNames.FirstName)?.Value;
			var lastName = claims.FirstOrDefault(x => x.Type == CustomClaimNames.LastName)?.Value;




			return new User
			{
				Id = userId,
				Email = userEmail,
				FirstName=firstName,
				LastName=lastName
			};
		}


	}
}
