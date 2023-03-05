using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Data;
using shopWebAPI.Models;
using shopWebAPI.Services;
using shopWebAPI.Utilities;

namespace shopWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string?  _connectionString;
	private readonly UserSqlConnection _sqlService;
    private readonly UserAuthenticationJWTService _jwtService;
	public UserController(IConfiguration configuration)
    {
        _connectionString = configuration[DatabaseConfigurations.DATABASE_CONFIG_DEFAULT];
		_sqlService = new UserSqlConnection(_connectionString);
		_jwtService = new UserAuthenticationJWTService(configuration);

	}
    // GET
    [HttpGet("GetUser")]
    public IActionResult GetUser()
    {
        return Ok();
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody]User user)
    {
		var getUser = await _sqlService.Select(user);

		if (getUser != null)
		{
			var token = _jwtService.CreateToken(user);
			return Ok(token);
		}
		else
		{
			return BadRequest("User not found");
		}
	}

    [HttpPost("Register")]
	public async Task<IActionResult> Register([FromBody] Register register)
    {
        return Ok();
    }
}