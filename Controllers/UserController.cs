using twitter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using twitter.Utilities;
using twitter.DTOs;
using twitter.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Todo.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _memoryCache;


    public UserController(ILogger<UserController> logger,
    IUserRepository user, IConfiguration config, IMemoryCache memoryCache)
    {
        _logger = logger;
        _user = user;
        _config = config;
        _memoryCache = memoryCache;

    }
    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == twitterConstants.UserId).First().Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResDTO>> Login(
        [FromBody] UserLoginDTO Data
    )
    {
        var userCache = _memoryCache.Get<User>(key: "user");
        if (userCache is null)
        {
            userCache = await _user.GetByEmail(Data.Email);
        }
        if (userCache is null)
            return NotFound();

        try
        {
            bool passwordVerify = BCrypt.Net.BCrypt.Verify(Data.Password, userCache.Password);

            if (!passwordVerify)
                return BadRequest("Incorrect password");

            var token = Generate(userCache);

            var res = new UserLoginResDTO
            {
                UserId = userCache.UserId,
                Email = userCache.Email,
                Token = token,
                FullName = userCache.Fullname
            };
            return Ok(res);
        }
        catch (Exception e)
        {
            Console.WriteLine(" error while verifying password: " + e.ToString());
            return Ok(" error while verifying password: " + e.ToString());
        }

    }

    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(twitterConstants.UserId, user.UserId.ToString()),
            new Claim(twitterConstants.Email, user.Email),
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UserCreateDto Data)
    {
        var toCreateuser = new User
        {
            Fullname = Data.Fullname.Trim(),
            Email = Data.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(Data.Password.Trim()),


        };

        var res = await _user.Create(toCreateuser);

        return StatusCode(StatusCodes.Status201Created, res);
    }


    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(
    [FromBody] UserUpdateDto Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _user.GetById(userId);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(403, "You cannot update other's TODO");

        var toUpdateItem = existingItem with
        {
            Fullname = Data.Fullname is null ? existingItem.Fullname : Data.Fullname.Trim(),

        };

        await _user.Update(toUpdateItem);

        return NoContent();
    }

    


}
