using WebApiKolokwium.Models;

namespace WebApiKolokwium.Repositories;

public interface IBookRepository
{
    Task<bool> ExistsBook(int id);
    Task<GetBookDTO> GetAuthors(int id);
    Task<bool> ExistsAuthor(AuthorDTO bookAuthors);
    Task<GetBookDTO> AddBook(AddBookDTO book);
}