using twitter.Models;
using Dapper;
using twitter.Utilities;


namespace twitter.Repositories;

public interface ICommentRepository
{
    Task<Comment> Create(Comment Item);

    Task Delete(long Id);
    Task<List<Comment>> GetAll();

    Task<Comment> GetById(long Id);

}

public class CommentRepository : BaseRepository, ICommentRepository
{
    public CommentRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<Comment> Create(Comment Item)
    {
        var query = $@"INSERT INTO {TableNames.comment} (post_id, user_id, text, created_at, updated_at) 
        VALUES (@PostId, @UserId, @Text, @CreatedAt, @UpdatedAt) RETURNING *";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, Item);
    }

    public async Task Delete(long Id)
    {
        var query = $@"DELETE FROM {TableNames.comment} WHERE comment_id = @CommentId";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { Id });
    }

    public async Task<List<Comment>> GetAll()
    {
        var query = $@"SELECT * FROM {TableNames.comment} ORDER BY created_at DESC";

        using (var con = NewConnection)
            return (await con.QueryAsync<Comment>(query)).AsList();
    }


    public async Task<Comment> GetById(long CommentId)
    {
        var query = $@"SELECT * FROM {TableNames.comment} WHERE comment_id = @CommentId";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Comment>(query, new { CommentId });
    }

}
