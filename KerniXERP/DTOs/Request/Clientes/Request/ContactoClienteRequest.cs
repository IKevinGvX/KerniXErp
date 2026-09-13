using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Clientes.Requests;

public sealed class ContactoClienteRequest
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

    public bool Estado { get; set; } = true;
}