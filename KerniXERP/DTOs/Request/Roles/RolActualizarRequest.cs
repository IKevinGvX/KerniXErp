using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Roles.Requests;

public sealed class RolActualizarRequest
{
    [Required]
    public int IdEmpresa { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public bool EsAdministrador { get; set; }

    public bool Estado { get; set; }
}