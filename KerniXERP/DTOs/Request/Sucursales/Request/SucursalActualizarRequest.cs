using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Sucursales.Requests;

public sealed class SucursalActualizarRequest
{
    [Required]
    public int IdEmpresa { get; set; }

    [Required]
    [StringLength(30)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Direccion { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(6)]
    public string? Ubigeo { get; set; }

    public bool Estado { get; set; }
}