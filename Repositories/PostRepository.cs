using twitter.Models;
using Dapper;
using twitter.Utilities;

namespace twitter.Repositories;

public interface IPostRepository
{
    Task<Post> Create(Post Item);
    Task Update(Post Item);
    Task Delete(long Id);
    Task<List<Post>> GetAll();
    Task<Post> GetById(long Id);

    Task<List<Post>> GetTweetsByUserId(long UserId);
}

public class PostRepository : BaseRepository, IPostRepository
{
    public PostRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<Post> Create(Post Item)
    {
        var query = $@"INSERT INTO {TableNames.post} (title, user_id, created_at, updated_at) 
        VALUES (@Title, @UserId, @CreatedAt, @UpdatedAt) RETURNING *";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, Item);
    }

    public async Task Delete(long PostId)
    {
        var query = $@"DELETE FROM {TableNames.post} WHERE post_id = @PostId";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, new { PostId });
    }

    public async Task<List<Post>> GetAll()
    {
        var query = $@"SELECT * FROM {TableNames.post} ORDER BY created_at DESC";

        using (var con = NewConnection)
            return (await con.QueryAsync<Post>(query)).AsList();
    }

    public async Task<Post> GetById(long PostId)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE post_id = @PostId";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<Post>(query, new { PostId });
    }

    public async Task Update(Post Item)
    {
        var query = $@"UPDATE {TableNames.post} SET title = @Title, 
        user_id = @UserId WHERE post_id = @PostId";

        using (var con = NewConnection)
            await con.ExecuteAsync(query, Item);
    }

    public async Task<List<Post>> GetTweetsByUserId(long UserId)
    {
        var query = $@"SELECT * FROM {TableNames.post} WHERE user_id = @UserId";

        using (var con = NewConnection)
            return (await con.QueryAsync<Post>(query, new { UserId })).AsList();
    }

}