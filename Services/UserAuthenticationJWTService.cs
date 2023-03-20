using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using shopWebAPI.Data;
using shopWebAPI.Models;
using shopWebAPI.Models.Jwt;
using shopWebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using static shopWebAPI.Utilities.DatabaseConfigurations;
namespace shopWebAPI.Services
{
	public class UserAuthenticationJWTService
	{
		private readonly string _secret;
		private readonly SymmetricSecurityKey _key;
		private readonly IConfiguration _configuration;

		private readonly CartSqlConnection _cartSqlConnection;
		public UserAuthenticationJWTService(IConfiguration configuration) {
			_configuration = configuration;
			_secret = configuration["Jwt:Secret"]??string.Empty;
			_key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
			_cartSqlConnection = new CartSqlConnection(configuration[DATABASE_CONFIG_DEFAULT]);


		}

		public JWTToken CreateToken(User user, RefreshToken? refreshToken=null)
		{

			var cart = _cartSqlConnection.SelectNonAsync(user.Id);

			var claims = new Dictionary<string, object>() {
				
				{ CustomClaimNames.Id,user.Id},
				{ CustomClaimNames.FirstName,user.FirstName??string.Empty},
				{ CustomClaimNames.LastName,user.LastName??string.Empty},
				{ ClaimTypes.Email,user.Email??string.Empty},
				{ ClaimTypes.Role,-1},
				{ CustomClaimNames.CartId,(cart is null ? -1 :cart.Id)},/* Todo get the user's cart/orderid */
				
			};

			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
			var handler = new JwtSecurityTokenHandler();

			var token = handler.CreateJwtSecurityToken(
				new SecurityTokenDescriptor
				{
					Claims= claims,
					Expires = DateTime.UtcNow.AddSeconds(20),
					SigningCredentials = creds

				}
				);

			var obj = handler.WriteToken(token);
			var refToken = new RefreshTokenGenerator();

			var jwtObj = new JWTToken
			{
				Token = obj,
				RefreshToken = 
				refreshToken is null ? 
				new RefreshToken 
				{ 
					RToken = refToken.GenerateRefreshToken(),
					ExpiringDate= DateTime.UtcNow.AddDays(2),
					FkUserId= user.Id,
					Key=Guid.NewGuid().ToString()
					
				}
				: refreshToken
				

			};


			
			return jwtObj;
		}
		public TokenValidationParameters GetTokenValidatorParams(bool expire = true)
		{
			return new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _key,

				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = expire,

				RequireExpirationTime = expire,
				
				ClockSkew = TimeSpan.Zero


			};
		}


		

	}
}
