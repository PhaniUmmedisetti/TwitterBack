using twitter.Models;
using twitter.Repositories;
using Microsoft.AspNetCore.Mvc;
using twitter.DTOs;
using Microsoft.AspNetCore.Authorization;
using twitter.Utilities;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace Twitter.Controllers;

[ApiController]
[Authorize]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;
    private readonly IMemoryCache _memoryCache;


    public CommentController(ILogger<CommentController> logger,
    ICommentRepository comment, IMemoryCache memoryCache)
    {
        _logger = logger;
        _comment = comment;
        _memoryCache = memoryCache;

    }

    private int GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == twitterConstants.UserId).First().Value);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment([FromBody] CommentCreateDTO Data)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var toCreateItem = new Comment
        {
            Text = Data.Text.Trim(),
            UserId = userId,
            PostId = Data.PostId,


        };


        var createdItem = await _comment.Create(toCreateItem);


        return StatusCode(201, createdItem);
    }



    [HttpDelete("{Comment_id}")]
    public async Task<ActionResult> DeleteComment([FromRoute] int comment_id)
    {
        var userId = GetUserIdFromClaims(User.Claims);

        var existingItem = await _comment.GetById(comment_id);

        if (existingItem is null)
            return NotFound();

        if (existingItem.UserId != userId)
            return StatusCode(403, "You cannot delete other's comment");

        await _comment.Delete(comment_id);

        return NoContent();
    }

    [HttpGet("{post_id}")]
    public async Task<ActionResult<List<Comment>>> GetAllComments([FromRoute] long post_id)
    {
        var allComment = await _comment.GetAll();
        return Ok(allComment);
    }
    [HttpGet]
    public async Task<ActionResult<List<Comment>>> GetAll([FromQuery] CommentParameters commentParameters)
    {
        var allComment = await _comment.GetAll(commentParameters);
        var commentCache = _memoryCache.Get<List<Comment>>(key: "comments");
        if (commentCache is null)
        {
            commentCache = await _comment.GetAll(commentParameters);
            _memoryCache.Set(key: "comments", commentCache, TimeSpan.FromMinutes(value: 1));
        }
        return Ok(allComment);
    }
}