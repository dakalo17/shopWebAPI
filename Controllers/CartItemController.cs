using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using shopWebAPI.Data;
using shopWebAPI.Models.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using shopWebAPI.Models;
using shopWebAPI.Services;
using shopWebAPI.Utilities;
using static shopWebAPI.Utilities.DatabaseConfigurations;


namespace shopWebAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CartItemController : ControllerBase
	{
		private readonly string? _connectionString;
		private readonly OrderSqlConnection _service;
		private StringValues? _token = null;
		private readonly IConfiguration _configuration;


		public CartItemController(IConfiguration configuration)
		{
			_connectionString = configuration[DATABASE_CONFIG_DEFAULT];
			_service = new OrderSqlConnection(_connectionString);
			_configuration = configuration;
		}

		[HttpGet("GetCartItem")]
		public async Task<IActionResult> GetCartItem()
		{

			return Ok();
		}

		[HttpGet("GetCartItems")]
		public async Task<IActionResult> GetCartItems()
		{
			_token = Request.Headers["Authorization"];
			if (_token is null) return BadRequest("Unexpected Internal error");


			var handler = new JwtSecurityTokenHandler();
			ClaimsPrincipal? principal = null;
			try
			{
				var tokenParams = new UserAuthenticationJWTService(_configuration)
					.GetTokenValidatorParams();
				
				
				principal = handler.ValidateToken(_token,
					tokenParams, out var validatedToken);
			}
			catch (SecurityTokenExpiredException)
			{
					return BadRequest("Token has expired");
		
			}
			catch (SecurityTokenNoExpirationException)
			{
				return BadRequest("Token has no expiration date");
			}
			catch (Exception ex)
			{
				ex.GetBaseException();
				return BadRequest("Internal error");
			}

			

			var userId = Convert.ToInt32(principal?.FindFirstValue(CustomClaimNames.Id));
			
			var cartItems = await _service.Select(userId);


			


			return Ok();
		}

	

		[HttpPost("PostCartItem")]
		public async Task<IActionResult> PostCartItem()
		{

			return Ok();
		}
		
		[HttpPut("PutCartItem")]
		public async Task<IActionResult> PutCartItem()
		{

			return Ok();
		}
	}
}
