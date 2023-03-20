using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using shopWebAPI.Data;
using shopWebAPI.Models;
using shopWebAPI.Models.Jwt;
using shopWebAPI.Repository.Jwt;
using shopWebAPI.Services;
using shopWebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace shopWebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string?  _connectionString;
	private readonly UserSqlConnection _sqlService;
    private readonly UserAuthenticationJWTService _jwtService;
	private IUserRefreshTokenRepository _userRepo;
	private readonly RefreshTokenSqlConnection _refreshTokenSql;
	private IConfiguration _configuration;


	public UserController(IConfiguration configuration,IUserRefreshTokenRepository userRepo)
    {
        _connectionString = configuration[DatabaseConfigurations.DATABASE_CONFIG_DEFAULT];
		_sqlService = new UserSqlConnection(_connectionString);
		_jwtService = new UserAuthenticationJWTService(configuration);
		_userRepo = userRepo;
		_refreshTokenSql = new RefreshTokenSqlConnection(_connectionString);
		_configuration = configuration;

	}
    // GET
    [HttpGet("GetUser")]
    public IActionResult GetUser()
    {
        return Ok();
    }

	[AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody]User user)
    {
		var getUser = await _sqlService.Select(user);

		if (getUser != null)
		{
			var tokenObj = _jwtService.CreateToken(getUser);
			

			//store refresh token in cookie**/to database
			var rowsAffected = await _refreshTokenSql.Insert(tokenObj?.RefreshToken);
			

			return rowsAffected > 0 ? Ok(tokenObj) :BadRequest("Token Problem");
		}
	    
		return BadRequest("User not found");
		
	}

    [AllowAnonymous]
	[HttpPost("RefreshToken")]
	public async Task<IActionResult> RefreshToken([FromBody] JWTToken jwtToken)
	{
		if (jwtToken.RefreshToken == null)
		  return BadRequest("Invalid Token");


		if (jwtToken?.RefreshToken?.ExpiringDate <= DateTime.UtcNow) {
			//if refresh token has expired
			return Unauthorized("Refresh token expired,re-login");
		}


		var handler = new JwtSecurityTokenHandler();
		ClaimsPrincipal? principal = null;

		try
		{
			principal = handler.ValidateToken(jwtToken.Token,
				_jwtService.GetTokenValidatorParams(), out var validatedToken);
		}
		catch (SecurityTokenExpiredException)
		{
			//	return BadRequest("Token has expired");
			principal = handler.ValidateToken(jwtToken.Token,
					_jwtService.GetTokenValidatorParams(false), out var validatedToken);
		}
		catch (SecurityTokenNoExpirationException)
		{
			return BadRequest("Token has no expiration date");
		}
		catch (Exception ex)
		{
			ex.GetBaseException();
		}

		//var valid = GetPrincipalFromExpiredToken(jwtToken.Token);
		var username = principal?.FindFirstValue(ClaimTypes.Email);
		var firstname = principal?.FindFirstValue(CustomClaimNames.FirstName);
		var lastname = principal?.FindFirstValue(CustomClaimNames.LastName);
		var userId = Convert.ToInt32(principal?.FindFirstValue(CustomClaimNames.Id));



		//retrieve user's refresh token(from database) and compare with in coming token
		var getUserRefreshToken =await _refreshTokenSql.Select(userId,jwtToken?.RefreshToken?.Key??string.Empty);

		if (!Equals(getUserRefreshToken?.RToken, jwtToken?.RefreshToken?.RToken)) 
			return BadRequest("Invalid request");
		
		var newUser = new User
		{
			Email = username,
			FirstName = firstname,
			LastName = lastname,
			Id = userId
		};

		var newToken = _jwtService.CreateToken(newUser, jwtToken?.RefreshToken);



		//save refresh token to database

		return Ok(newToken);

	}



	[AllowAnonymous]
	[HttpPost("SignUp")]
	public async Task<IActionResult> SignUp([FromBody] Register register)
    {
        return Ok();
    }
}