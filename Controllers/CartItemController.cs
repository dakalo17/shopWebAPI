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
		private readonly CartSqlConnection _serviceCart;
		private readonly CartItemSqlConnection _serviceCartItem;
		private StringValues? _token = null;
		private readonly IConfiguration _configuration;

        public CartItemController(IConfiguration configuration)
		{
			_connectionString = configuration[DATABASE_CONFIG_DEFAULT];
            _serviceCart = new CartSqlConnection(_connectionString);
			_serviceCartItem = new CartItemSqlConnection(_connectionString);
			_configuration = configuration;

            

        }

		[HttpGet("GetCartItem")]
		public async Task<IActionResult> GetCartItem(int cartId)
		{
			
			var user = GetThisUser();

			if (user == null) return BadRequest(); 

			var cartItem = await _serviceCart.Select(user.Id,cartId);

			return Ok(cartItem);
		}

		//orderid == cartid
		[Obsolete]
		[HttpGet("GetCartItems/{cartId}")]
		public async Task<IActionResult> GetCartItems(int cartId)
		{

			//TODO user ID required
			var cartProducts = await _serviceCartItem.SelectOnOrder(cartId);

			
			return cartProducts is null ? NoContent():Ok(cartProducts);
		}


		[AllowAnonymous]
        [HttpGet("GetCartItems")]
        public async Task<IActionResult> GetCartItems()
        {
			var user = GetThisUser();


			if(user is null) return BadRequest();

            var cartProducts = await _serviceCartItem.SelectOnOrder(user.Id);


            return cartProducts is null ? NoContent() : Ok(cartProducts);
        }

        //[AllowAnonymous]

        [HttpPost("PostCartItem")]
		public async Task<IActionResult> PostCartItem([FromBody] CartItem cartItem)
		{

			var user = GetThisUser();
			
			if (user == null) return Unauthorized();

			var res = await _serviceCartItem.Insert(cartItem,user.Id);

			return res != null ? Ok(res) : Conflict(new CartProduct
			{
				Name = "ERROR",
				Image_link = "ERROR"
			});
		}
		[Obsolete("Use PostCartItem instead,its using Upseart")]
		[HttpPut("PutCartItem")]
		public async Task<IActionResult> PutCartItem([FromBody] CartItem cartItem)
		{

			//Todo edit
			//_serviceCartItem.
			var res = await _serviceCartItem.UpdateProduct(cartItem);
			return res is not null ? Ok(new AbstractResponse
			{
				Response = Convert.ToString(res)??"0"
			}): NotFound();
		}
		private User? GetThisUser()
		{
			var auth = Request.Headers["Authorization"];
			var token = auth[0]?.Split(" ")[1];
			return RefreshTokenGenerator.GetUserFromToken(token ?? string.Empty, _configuration);
		
		}
	}
}
