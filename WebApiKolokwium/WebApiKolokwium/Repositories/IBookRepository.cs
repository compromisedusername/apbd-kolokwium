using WebApiKolokwium.Models;

namespace WebApiKolokwium.Repositories;

public interface IBookRepository
{
    Task<bool> ExistsBook(int id);
    Task<BookDTO> GetAuthors(int id);
}