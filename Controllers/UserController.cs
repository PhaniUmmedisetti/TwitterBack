using twitter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using twitter.Utilities;
using twitter.DTOs;
using twitter.Repositories;

namespace Todo.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _user;
    private readonly IConfiguration _config;

    public UserController(ILogger<UserController> logger,
    IUserRepository user, IConfiguration config)
    {
        _logger = logger;
        _user = user;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResDTO>> Login(
        [FromBody] UserLoginDTO Data
    )

    {
        var existingUser = await _user.GetByEmail(Data.Email);

        if (existingUser is null)
            return NotFound();

        try
        {
            bool passwordVerify = BCrypt.Net.BCrypt.Verify(Data.Password, existingUser.Password);

            if (!passwordVerify)
                return BadRequest("Incorrect password");

            var token = Generate(existingUser);

            var res = new UserLoginResDTO
            {
                Id = existingUser.Id,
                Email = existingUser.Email,
                Token = token,
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
            new Claim(twitterConstants.Id, user.Id.ToString()),
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


    [HttpPut("{id}")]

    public async Task<ActionResult> UpdateUser([FromRoute] long id,
    [FromBody] UserUpdateDto Data)
    {
        var existing = await _user.GetById(id);
        if (existing is null)
            return NotFound("No user found with given id");

        var toUpdateUser = existing with
        {
            Email = Data.Email?.Trim()?.ToLower() ?? existing.Email,
            Password = Data.Password?.Trim()?.ToLower() ?? existing.Password,

        };

        var didUpdate = await _user.Update(toUpdateUser);

        if (!didUpdate)
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not update user");

        return NoContent();
    }


}
