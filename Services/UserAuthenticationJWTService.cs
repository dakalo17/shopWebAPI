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

namespace shopWebAPI.Services
{
	public class UserAuthenticationJWTService
	{
		private readonly string _secret;
		private readonly SymmetricSecurityKey _key;
		public UserAuthenticationJWTService(IConfiguration configuration) {
			_secret = configuration["Jwt:Secret"]??string.Empty;
			_key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));


		}

		public JWTToken CreateToken(User user)
		{
			

			var claims = new Dictionary<string, object>() {
				
				{ CustomClaimNames.Id,user.Id},
				{ CustomClaimNames.FirstName,user.FirstName??string.Empty},
				{ CustomClaimNames.LastName,user.LastName??string.Empty},
				{ CustomClaimNames.Email,user.Email??string.Empty},
				//this is temporary solution
				{ ClaimTypes.Name,user.Email??string.Empty},
				{ CustomClaimNames.Role,-1},
				
			};

			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
			var handler = new JwtSecurityTokenHandler();

			var token = handler.CreateJwtSecurityToken(
				new SecurityTokenDescriptor
				{
					Claims= claims,
					Expires = DateTime.UtcNow.AddSeconds(3000),
					SigningCredentials = creds

				}
				);

			var obj = handler.WriteToken(token);
			var refToken = new RefreshTokenGenerator();

			var jwtObj = new JWTToken
			{
				Token = obj,
				RefreshToken = refToken.GenerateRefreshToken()

			};


			
			return jwtObj;
		}
		public TokenValidationParameters GetTokenValidatorParams()
		{
			return new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _key,

				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,

				RequireExpirationTime = true,


			};
		}

	}
}
