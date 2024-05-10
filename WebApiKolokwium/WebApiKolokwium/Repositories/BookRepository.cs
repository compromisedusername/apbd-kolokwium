using System.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApiKolokwium.Models;

namespace WebApiKolokwium.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> ExistsBook(int id)
    {
        var query = @"SELECT 1 FROM Books WHERE [PK] = @IdBook";
        
        await using var  connection =  new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("IdBook", id);

        await connection.OpenAsync();
        
        var  res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<GetBookDTO> GetAuthors(int id)
    {
        var query = 
                  @"SELECT b.title as bookTitle, a.first_name as authorFirstName, a.last_name as authorLastName FROM Books b JOIN books_authors ba ON b.PK = b.PK
JOIN authors a ON a.PK = ba.FK_author";
        
        await using var  connection =  new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("bookId", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookTitle = reader.GetOrdinal("bookTitle");
        var authorFirstName = reader.GetOrdinal("authorFirstName");
        var authorLastName = reader.GetOrdinal("authorLastName");
        
        

        GetBookDTO getBook = null;
        
        while (await reader.ReadAsync())
        {
            if (getBook is not null)
            {
                getBook.Authors.Add(new AuthorDTO()
                {
                    LastName = reader.GetString(authorLastName),
                    FirstName = reader.GetString(authorFirstName)
                });
            }
            else
            {
                getBook = new GetBookDTO()
                {
                    Id = id,
                    Title = reader.GetString(bookTitle),
                    Authors = new List<AuthorDTO>
                    {
                        new()
                        {
                            LastName = reader.GetString(authorLastName),
                            FirstName = reader.GetString(authorFirstName),
                        }
                    }
                };
            }
        }

        if (getBook is null)
        {
            throw new Exception("Book doesnt exists!");
        }
        
        return getBook;

    }

    public async Task<bool> ExistsAuthor(AuthorDTO bookAuthors)
    {
        var query = @"SELECT 1 FROM Authors WHERE first_name = @authorFirstName AND last_name = @authorLastName";
        
        await using var  connection =  new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        
        command.CommandText = query;
        command.Connection = connection;
        
        command.Parameters.AddWithValue("authorLastName", bookAuthors.LastName);
        command.Parameters.AddWithValue("authorFirstName", bookAuthors.FirstName);

        await connection.OpenAsync();
        
        var  res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<GetBookDTO> AddBook(AddBookDTO addBookDto)
    {
        var insert = @"INSERT INTO Books VALUES (@bookTitle) SELECT @@IDENTITY AS ID;";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = insert;
        
        command.Parameters.AddWithValue("bookTitle", addBookDto.Title);

        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;


        try
        {
            var idBook = await command.ExecuteScalarAsync();
            foreach (var author in addBookDto.Authors)
            {
                command.Parameters.Clear();
                var select = @"SELECT PK FROM Authors WHERE first_name = @authorFirstName AND last_name = @authorLastName";
                command.CommandText = select;
                command.Parameters.AddWithValue("authorLastName", author.LastName);
                command.Parameters.AddWithValue("authorFirstName", author.FirstName);
                var res = await command.ExecuteScalarAsync();
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO books_authors VALUES (@BookId,@AuthorId)";
                command.Parameters.AddWithValue("BookId", idBook);
                command.Parameters.AddWithValue("AuthorId", res );
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return new GetBookDTO() { Authors = addBookDto.Authors, Id =(int)((decimal)idBook), Title = addBookDto.Title };

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }


    }
}