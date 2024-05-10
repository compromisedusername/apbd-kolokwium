using System.ComponentModel.DataAnnotations;

namespace WebApiKolokwium.Models;

public class AuthorDTO
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}