using twitter.Models;
using twitter.Repositories;
using Microsoft.AspNetCore.Mvc;
using twitter.DTOs;
using Microsoft.AspNetCore.Authorization;
using twitter.Utilities;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Todo.Controllers;

[ApiController]
[Authorize]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _post;
    private readonly IMemoryCache _memoryCache;


    public PostController(ILogger<PostController> logger,
    IPostRepository post, IMemoryCache memoryCache
    )
    {
        _logger = logger;
        _post = post;
        _memoryCache = memoryCache;

    }
    // {
    //     _logger = logger;
    //     _post = post;
    // }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == twitterConstants.UserId).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreateTweet([FromBody] PostCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);



        List<Post> usertweets = await _post.GetTweetsByUserId(userId);
        if (usertweets != null && usertweets.Count >= 5)
        {
            return BadRequest("Limit exceeded");
        }

        var toCreateItem = new Post
        {
            Title = Data.Title.Trim(),
            UserId = userId,

        };


        var createdItem = await _post.Create(toCreateItem);


        return StatusCode(201, createdItem);
    }


    [HttpPut("{post_id}")]
    public async Task<ActionResult> UpdateTodo([FromRoute] long post_id,
    [FromBody] PostUpdateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _post.GetById(post_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot update other's post");

        var toUpdateItem = existingItem with
        {
            Title = Data.Title is null ? existingItem.Title : Data.Title.Trim(),

        };

        await _post.Update(toUpdateItem);

        return NoContent();
    }

    [HttpDelete("{post_id}")]
    public async Task<ActionResult> DeleteTodo([FromRoute] long post_id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _post.GetById(post_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(404, "You cannot delete other's POST");

        await _post.Delete(post_id);

        return NoContent();
    }



    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetAllPosts([FromQuery] PostParameters postParameters)
    {
        var postCache = _memoryCache.Get<List<Post>>(key: "posts");
        if (postCache is null)
        {
            postCache = await _post.GetAll(postParameters);
            _memoryCache.Set(key: "posts", postCache, TimeSpan.FromMinutes(value: 1));
        }



        return Ok(postCache);
        // return Ok(allPosts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<Post>>> GetPost([FromRoute] long id)
    {
        var post = await _post.GetPost(id);
        return Ok(post);
    }
    // {
    //     var allPosts = await _post.GetPost(Id);
    //     return Ok(allPosts);
    // }
}