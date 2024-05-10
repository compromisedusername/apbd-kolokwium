using System.ComponentModel.DataAnnotations;

namespace WebApiKolokwium.Models;

public class AuthorDTO
{
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string FirstName { get; set; }
    [Required]
    
    [MinLength(1)]
    [MaxLength(100)]
    public string LastName { get; set; }
}