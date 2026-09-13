using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Usuarios.Requests;

public sealed class UsuarioRolesRequest
{
    [Required]
    [MinLength(1)]
    public List<int> IdRoles { get; set; } = [];
}