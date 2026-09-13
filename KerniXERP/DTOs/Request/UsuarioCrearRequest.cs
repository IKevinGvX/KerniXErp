using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Usuarios.Requests;

public sealed class UsuarioCrearRequest
{
    [Required]
    public int IdEmpleado { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public List<int> Roles { get; set; } = new();

    public bool RequiereCambioClave { get; set; } = true;
}