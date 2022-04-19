using twitter.Models;
using Dapper;
using twitter.Utilities;

namespace twitter.Repositories;

public interface IUserRepository
{
    Task<User> GetByEmail(string Email);

    Task<User> Create(User Item);

    Task<bool> Update(User Item);

    Task<User> GetById(long UserId);
}
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration config) : base(config)
    {

    }

    public async Task<User> GetByEmail(string Email)
    {
        var query = $@"SELECT * FROM ""{TableNames.user}"" 
        WHERE Email = @Email";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query, new { Email });
    }


    public async Task<User> Create(User Item)
    {
        var query = $@"INSERT INTO ""{TableNames.user}"" (full_name, password, email) 
       VALUES (@Fullname, @Password, @Email)
       RETURNING *";


        using (var con = NewConnection)
        {
            var res = await con.QuerySingleOrDefaultAsync<User>(query, Item);
            return res;
        }
    }

    public async Task<bool> Update(User Item)
    {
        var query = $@"UPDATE ""{TableNames.user}"" SET password = @Password,email = @Email WHERE id = Id";


        using (var con = NewConnection)
        {
            var rowCount = await con.ExecuteAsync(query, Item);

            return rowCount == 1;
        }
    }

    public async Task<User> GetById(long Id)
    {
        var query = $@"SELECT * FROM ""{TableNames.user}""
        WHERE id = @Id";

        using (var con = NewConnection)
            return await con.QuerySingleOrDefaultAsync<User>(query,
            new
            {
                Id = Id
            });
    }

}