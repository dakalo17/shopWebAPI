using Microsoft.AspNetCore.Mvc;
using shopWebAPI.Models;

namespace shopWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly string?  _connectionString;
    public UserController(IConfiguration configuration)
    {
        _connectionString = configuration["DefaultConnection"];
    }
    // GET
    [HttpGet("GetUser")]
    public IActionResult GetUser()
    {
        return Ok();
    }


    [HttpPost("Login")]
    public IActionResult Login([FromBody]User user)
    {
        return Ok();
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] Register register)
    {
        return Ok();
    }
}