using Microsoft.AspNetCore.Mvc;
using WebApiKolokwium.Models;
using WebApiKolokwium.Repositories;

namespace WebApiKolokwium.Controllers;

[Route("api/books")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet("{id}/authors")]
    public async Task<IActionResult> GetAuthors(int id)
    {
        if (!await _bookRepository.ExistsBook(id))
        {
            return NotFound(new ExceptionDTO(){Message = "Book for id "+id+" doesnt exists!", StatusCode = 404});
        }

        try
        {
            var res = await _bookRepository.GetAuthors(id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(new ExceptionDTO(){Message = "Book doesnt exists!", StatusCode = 400 })
        }

    }

    [HttpPost]
    public async Task<IActionResult> AddBook(AddBookDTO book)
    {
        foreach (var author in book.Authors)
        {
            if (!await _bookRepository.ExistsAuthor(author))
                return NotFound(new ExceptionDTO() { Message = "Author not found!", StatusCode = 400 });
        }

        var res = await _bookRepository.AddBook(book);
        return StatusCode(201,res);
}
}