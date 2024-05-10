using System.Text.Json;

namespace WebApiKolokwium.Models;

public class ExceptionDTO
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}