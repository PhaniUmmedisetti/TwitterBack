using twitter.Models;
using twitter.Repositories;
using Microsoft.AspNetCore.Mvc;
using twitter.DTOs;
using Microsoft.AspNetCore.Authorization;
using twitter.Utilities;
using System.Security.Claims;


namespace Twitter.Controllers;

[ApiController]
[Authorize]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentRepository _comment;

    public CommentController(ILogger<CommentController> logger,
    ICommentRepository comment)
    {
        _logger = logger;
        _comment = comment;
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

    [HttpGet]
    public async Task<ActionResult<List<Comment>>> GetAllComments()
    {
        var allComment = await _comment.GetAll();
        return Ok(allComment);
    }
}