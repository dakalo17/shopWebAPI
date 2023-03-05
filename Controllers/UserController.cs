using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Data;
using shopWebAPI.Models;
using shopWebAPI.Models.Jwt;
using shopWebAPI.Repository.Jwt;
using shopWebAPI.Services;
using shopWebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;

namespace shopWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string?  _connectionString;
	private readonly UserSqlConnection _sqlService;
    private readonly UserAuthenticationJWTService _jwtService;
	private IUserRefreshTokenRepository _userRepo;
	public UserController(IConfiguration configuration,IUserRefreshTokenRepository userRepo)
    {
        _connectionString = configuration[DatabaseConfigurations.DATABASE_CONFIG_DEFAULT];
		_sqlService = new UserSqlConnection(_connectionString);
		_jwtService = new UserAuthenticationJWTService(configuration);
		_userRepo = userRepo;

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
			var tokenObj = _jwtService.CreateToken(user);

			_userRepo.SaveRefreshToken(new UserRefreshToken
			{
				RefreshToken = tokenObj.RefreshToken,
				Username = user.Email??string.Empty
			});
			return Ok(tokenObj);
		}
		else
		{
			return BadRequest("User not found");
		}
	}


	[HttpPost("RefreshToken")]
	public async Task<IActionResult> RefreshToken([FromBody] JWTToken jwtToken)
	{
		if (jwtToken is null) return BadRequest();
		var handler = new JwtSecurityTokenHandler();

		IPrincipal principal = handler.ValidateToken(jwtToken.Token,
			_jwtService.GetTokenValidatorParams(), out var validatedToken);

		var username = principal?.Identity?.Name;
	}
	[HttpPost("Register")]
	public async Task<IActionResult> Register([FromBody] Register register)
    {
        return Ok();
    }
}