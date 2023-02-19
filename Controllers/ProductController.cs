using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Data;
using shopWebAPI.Models;
using System.Net;
using static shopWebAPI.Utilities.DatabaseConfigurations;

namespace shopWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly string? _connectionString;
		private readonly ProductSqlConnection _service;
		public ProductController(IConfiguration configuration)
		{
			_connectionString = configuration[DATABASE_CONFIG_DEFAULT];
			_service = new ProductSqlConnection(_connectionString);
		}

		

		[HttpGet("GetProducts")]
		public async Task<IActionResult> GetProducts()
		{
			var products = await _service.Select();

			return products is not null ? Ok(products):NoContent();
		}

		[HttpGet("GetProduct/{productName}")]
		public async Task<IActionResult> GetProduct(string productName)
		{
			var product = await _service.Select(productName);

			return product is not null ? Ok(product) : NoContent();
		}

		[HttpGet("GetProduct/{Id}")]
		public async Task<IActionResult> GetProduct(int Id)
		{
			var product = await _service.Select(Id);

			return product is not null ? Ok(product) : NoContent();
		}

		[HttpPost("PostProduct")]
		public async Task<IActionResult> PostProduct([FromBody] Product product)
		{
			var rowsAffected = await _service.Insert(product);

			return rowsAffected > 0 ? Ok(rowsAffected) :Conflict() ;
		}

		[HttpPut("PutProduct")]
		public async Task<IActionResult> PutProduct([FromBody] Product product){
			
			var rowsAffected = await _service.Put(product);

			
			return rowsAffected > 0 ? Ok(rowsAffected) : Conflict();

		}
	}
}
