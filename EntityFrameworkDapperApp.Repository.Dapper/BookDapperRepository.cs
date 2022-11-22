using System.Data;
using Dapper;
using EntityFrameworkDapperApp.Core.Contracts.Repositories;
using EntityFrameworkDapperApp.Core.Entities;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Core.Exceptions;
using EntityFrameworkDapperApp.Repository.Dapper.Schemas;

namespace EntityFrameworkDapperApp.Repository.Dapper;

public class BookDapperRepository : IBookDapperRepository
{
    private readonly DapperDbContext _context;

    public BookDapperRepository(DapperDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetAll()
    {
        return await FetchData();
    }

    public async Task<BookDto?> GetById(Guid id)
    {
        var result = await FetchData(id);

        return result.SingleOrDefault();
    }

    public async Task Create(Book item)
    {
        const string query =
            $@"INSERT INTO ""{BookSchema.Table}"" (""{BookSchema.Columns.Id}"", ""{BookSchema.Columns.Title}"", ""{BookSchema.Columns.PublishedOn}"") " +
            $@"VALUES (@{BookSchema.Columns.Id}, @{BookSchema.Columns.Title}, @{BookSchema.Columns.PublishedOn})";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, item.Id, DbType.Guid);
        parameters.Add(BookSchema.Columns.Title, item.Title, DbType.String);
        parameters.Add(BookSchema.Columns.PublishedOn, item.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
            throw new Exception("The resource was not modified.");
    }

    public async Task Update(Book item)
    {
        const string query =
            $@"UPDATE ""{BookSchema.Table}"" SET " +
            $@"""{BookSchema.Columns.Title}"" = @{BookSchema.Columns.Title}, " +
            $@"""{BookSchema.Columns.PublishedOn}"" = @{BookSchema.Columns.PublishedOn} " +
            $@"WHERE ""{BookSchema.Columns.Id}"" = @{BookSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, item.Id, DbType.Guid);
        parameters.Add(BookSchema.Columns.Title, item.Title, DbType.String);
        parameters.Add(BookSchema.Columns.PublishedOn, item.PublishedOn, DbType.DateTime);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(item.Id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(item.Id);
            else
                throw new Exception("The resource was not modified.");
        }
    }

    public async Task Remove(Guid id)
    {
        const string query =
            $@"DELETE FROM ""{BookSchema.Table}"" WHERE ""{BookSchema.Columns.Id}"" = @{BookSchema.Columns.Id}";

        var parameters = new DynamicParameters();
        parameters.Add(BookSchema.Columns.Id, id, DbType.Guid);

        using var connection = _context.CreateConnection();
        var result = await connection.ExecuteAsync(query, parameters);

        if (result == 0)
        {
            var itemExists = await ItemExists(id, connection);
            if (!itemExists)
                throw new EntityNotFoundException(id);
            else
                throw new Exception("The resource was not modified.");
        }
    }

    public async Task<bool> ItemExists(Guid id)
    {
        using var connection = _context.CreateConnection();
        return await ItemExists(id, connection);
    }

    private async Task<bool> ItemExists(Guid id, IDbConnection connection)
    {
        var sql = $@"SELECT COUNT(1) FROM ""{BookSchema.Table}"" WHERE ""{BookSchema.Columns.Id}"" = '{id}'";
        return await connection.ExecuteScalarAsync<bool>(sql);
    }

    private async Task<IEnumerable<BookDto>> FetchData(Guid? itemId = null)
    {
        const string baseQuery =
            $@"SELECT " +
            $@"b.""{BookSchema.Columns.Id}"", " +
            $@"b.""{BookSchema.Columns.Title}"", " +
            $@"b.""{BookSchema.Columns.PublishedOn}"", " +
            $@"r.""{ReviewSchema.Columns.Id}"", " +
            $@"r.""{ReviewSchema.Columns.Comment}"", " +
            $@"r.""{ReviewSchema.Columns.Rating}"" " +
            $@"FROM ""{BookSchema.Table}"" b " +
            $@"LEFT JOIN ""{ReviewSchema.Table}"" r ON b.""{BookSchema.Columns.Id}"" = r.""{ReviewSchema.Columns.BookId}""";

        using var connection = _context.CreateConnection();
        var result = new Dictionary<Guid, BookDto>();
        await connection.QueryAsync<Book, Review, Book>(
            itemId == null ? baseQuery : $@"{baseQuery} WHERE b.""{BookSchema.Columns.Id}"" = '{itemId.Value}'",
            (book, review) =>
            {
                // Check if the item was already added just to append a comment
                if (result.TryGetValue(book.Id, out var existingItem))
                {
                    existingItem.Reviews.Append(new ReviewBaseDto
                        { Id = review.Id, Comment = review.Comment, Rating = review.Rating });
                }
                // Create a new Book item and add it to the dictionary
                else
                {
                    var newItem = new BookDto { Id = book.Id, Title = book.Title, PublishedOn = book.PublishedOn };
                    if (review != null)
                        newItem.Reviews.Append(new ReviewBaseDto
                            { Id = review.Id, Comment = review.Comment, Rating = review.Rating });
                    result.Add(book.Id, newItem);
                }

                return book;
            }
        );

        return result.Values;
    }
}