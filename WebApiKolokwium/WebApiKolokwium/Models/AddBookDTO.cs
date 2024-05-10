using System.ComponentModel.DataAnnotations;

namespace WebApiKolokwium.Models;

public class AddBookDTO
{
    [Required]
    public string Title { get; set; }
    [Required]
    public List<AuthorDTO> Authors { get; set; }
}