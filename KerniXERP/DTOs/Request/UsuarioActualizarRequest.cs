using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Usuarios.Requests;

public sealed class UsuarioActualizarRequest
{
    [Required]
    public int IdEmpleado { get; set; }

    [Required]
    [StringLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    public bool Estado { get; set; }

    public bool RequiereCambioClave { get; set; }
}