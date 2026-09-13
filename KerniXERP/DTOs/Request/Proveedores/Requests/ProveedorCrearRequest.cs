using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Proveedores.Requests;

public sealed class ProveedorCrearRequest
{
    [Range(1, int.MaxValue)]
    public int IdEmpresa { get; set; }

    [Required]
    [StringLength(40)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(400)]
    public string RazonSocial { get; set; } = string.Empty;

    [StringLength(300)]
    public string? NombreComercial { get; set; }

    [StringLength(40)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(300)]
    public string? Correo { get; set; }

    public List<ContactoProveedorRequest> Contactos { get; set; } = [];

    public List<DireccionProveedorRequest> Direcciones { get; set; } = [];
}