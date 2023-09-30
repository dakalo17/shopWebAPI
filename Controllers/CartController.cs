using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Data;

using shopWebAPI.Models;
using shopWebAPI.Utilities;
using static shopWebAPI.Utilities.DatabaseConfigurations;

namespace shopWebAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly string? _connectionString;
		private readonly CartSqlConnection _service;

		public CartController(IConfiguration configuration)
		{
			
			_connectionString = configuration[DATABASE_CONFIG_DEFAULT];
			_service = new CartSqlConnection(_connectionString);

		}

		[HttpPost("GetCart")]
		public async Task<IActionResult> GetCart()
		{
			
			return Ok();
		}
		
		[HttpPost("GetCarts")]
		public async Task<IActionResult> GetCarts()
		{
			var token = GetUserFromToken.GetToken(Request);



			//await _service.SelectAsync(
			var res = await _service.Select();
			return res is null ? NotFound():Ok(res);
		}
		
		[HttpPost("PostCart")]
		public async Task<IActionResult> PostCart([FromBody]Cart order)
		{
			
			return Ok();
		}

	}
}
