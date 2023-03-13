using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Data;
using shopWebAPI.Models;
using static shopWebAPI.Utilities.DatabaseConfigurations;

namespace shopWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly string? _connectionString;
		private readonly OrderProductSqlConnection _service;

		public CartController(IConfiguration configuration)
		{
			
			_connectionString = configuration[DATABASE_CONFIG_DEFAULT];
			_service = new OrderProductSqlConnection(_connectionString);

		}

		[HttpPost("GetCart")]
		public async Task<IActionResult> GetCart()
		{
			return Ok();
		}
		
		[HttpPost("GetCarts")]
		public async Task<IActionResult> GetCarts()
		{
			return Ok();
		}
		
		[HttpPost("PostCart")]
		public async Task<IActionResult> PostCart([FromBody]Order order)
		{
			
			return Ok();
		}

	}
}
