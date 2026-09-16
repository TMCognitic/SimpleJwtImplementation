using System.ComponentModel.DataAnnotations;

namespace SimpleJwtImplementation.Models.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
