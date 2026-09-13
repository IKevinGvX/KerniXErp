using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Proveedores.Requests;

public sealed class ContactoProveedorRequest
{
    [Required]
    [StringLength(300)]
    public string Nombres { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Cargo { get; set; }

    [StringLength(40)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(300)]
    public string? Correo { get; set; }

    public bool EsPrincipal { get; set; }
}