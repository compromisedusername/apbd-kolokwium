using System.ComponentModel.DataAnnotations;

namespace WebApiKolokwium.Models;

public class BookDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public List<AuthorDTO> Authors { get; set; }
}