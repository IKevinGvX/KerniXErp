using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Requests;

public sealed class RefreshTokenRequest
{
    [Required(ErrorMessage = "El refresh token es obligatorio.")]
    public string RefreshToken { get; set; } = string.Empty;
}