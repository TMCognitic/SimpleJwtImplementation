using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace SimpleJwtImplementation.Models.Dtos
{
    public class RefreshTokenDto
    {
        [Required]
        public string Token { get; set; } = default!;
        [Required]
        public string RefreshToken { get; set; } = default!;
    }
}
