using System.ComponentModel.DataAnnotations;

namespace WebApiKolokwium.Models;

public class AddBookDTO
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string Title { get; set; }
    [Required]
    public List<AuthorDTO> Authors { get; set; }
}