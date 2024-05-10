using System.Data.SqlClient;
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

    public async Task<BookDTO> GetAuthors(int id)
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
        
        

        BookDTO book = null;
        
        while (await reader.ReadAsync())
        {
            if (book is not null)
            {
                book.Authors.Add(new AuthorDTO()
                {
                    LastName = reader.GetString(authorLastName),
                    FirstName = reader.GetString(authorFirstName)
                });
            }
            else
            {
                book = new BookDTO()
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

        if (book is null)
        {
            throw new Exception("Book doesnt exists!");
        }
        
        return book;

    }
}