using System.Data;
using Dapper;
using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using EntityFrameworkDapperApp.Repository.Dapper.Schemas;

namespace EntityFrameworkDapperApp.Repository.Dapper;

public class ReviewDapperRepository : IReviewDapperRepository
{
    private readonly DapperDbContext _context;

    public ReviewDapperRepository(DapperDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReviewDto>> GetAll()
    {
        return await FetchData();
    }

    public async Task<ReviewDto?> GetById(Guid id)
    {
        var result = await FetchData(id);

        return result.SingleOrDefault();
    }

    public async Task Create(Review item)
    {
        const string sql =
            $@"INSERT INTO ""{ReviewSchema.Table}"" (" +
            $@"""{ReviewSchema.Columns.Id}"", " +
            $@"""{ReviewSchema.Columns.Comment}"", " +
            $@"""{ReviewSchema.Columns.Rating}"", " +
            $@"""{ReviewSchema.Columns.BookId}"") " +
            $@"VALUES (" +
            $@"@{ReviewSchema.Columns.Id}, " +
            $@"@{ReviewSchema.Columns.Comment}, " +
            $@"@{ReviewSchema.Columns.Rating}, " +
            $@"@{ReviewSchema.Columns.BookId})";

        var parameters = new DynamicParameters();
        parameters.Add(ReviewSchema.Columns.Id, item.Id, DbType.Guid);
        parameters.Add(ReviewSchema.Columns.Comment, item.Comment, DbType.String);
        parameters.Add(ReviewSchema.Columns.Rating, item.Rating, DbType.Int32);
        parameters.Add(ReviewSchema.Columns.BookId, item.BookId, DbType.Guid);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(sql, parameters);

        if (result == 0)
            throw new Exception("The resource was not created.");
    }

    public async Task Update(Review item)
    {
        const string sql =
            $@"UPDATE ""{ReviewSchema.Table}"" SET " +
            $@"""{ReviewSchema.Columns.Comment}"" = @{ReviewSchema.Columns.Comment}, " +
            $@"""{ReviewSchema.Columns.Rating}"" = @{ReviewSchema.Columns.Rating} " +
            $@"WHERE ""{ReviewSchema.Columns.Id}"" = @{ReviewSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(ReviewSchema.Columns.Id, item.Id, DbType.Guid);
        parameters.Add(ReviewSchema.Columns.Comment, item.Comment, DbType.String);
        parameters.Add(ReviewSchema.Columns.Rating, item.Rating, DbType.Int32);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(sql, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(item.Id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(item.Id);
            else
                throw new Exception("The resource was not updated.");
        }
    }

    public async Task Remove(Guid id)
    {
        const string sql =
            $@"DELETE FROM ""{ReviewSchema.Table}"" WHERE ""{ReviewSchema.Columns.Id}"" = @{ReviewSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, id, DbType.Guid);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(sql, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(id);
            else
                throw new Exception("The resource was not removed.");
        }
    }

    public async Task<bool> ItemExists(Guid id)
    {
        using var connection = _context.CreateConnection();
        return await ItemExists(id, connection);
    }

    private async Task<bool> ItemExists(Guid id, IDbConnection connection)
    {
        var sql = $@"SELECT COUNT(1) FROM ""{ReviewSchema.Table}"" WHERE ""{ReviewSchema.Columns.Id}"" = '{id}'";
        return await connection.ExecuteScalarAsync<bool>(sql);
    }

    private async Task<IEnumerable<ReviewDto>> FetchData(Guid? itemId = null)
    {
        const string baseSql =
            $@"SELECT " +
            $@"r.""{ReviewSchema.Columns.Id}"", " +
            $@"r.""{ReviewSchema.Columns.Comment}"", " +
            $@"r.""{ReviewSchema.Columns.Rating}"", " +
            $@"r.""{ReviewSchema.Columns.BookId}"" " +
            $@"FROM ""{ReviewSchema.Table}"" r";

        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<ReviewDto>(
            itemId == null ? baseSql : $@"{baseSql} WHERE r.""{ReviewSchema.Columns.Id}"" = '{itemId.Value}'"
        );

        return result;
    }
}