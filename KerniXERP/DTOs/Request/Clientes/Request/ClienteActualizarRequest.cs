using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Clientes.Requests;

public sealed class ClienteActualizarRequest
{
    [Range(1, int.MaxValue)]
    public int IdEmpresa { get; set; }

    [Required]
    [StringLength(40)]
    public string TipoCliente { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [StringLength(400)]
    public string? RazonSocial { get; set; }

    [StringLength(200)]
    public string? Nombres { get; set; }

    [StringLength(300)]
    public string? Apellidos { get; set; }

    [StringLength(40)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(300)]
    public string? Correo { get; set; }

    public bool Estado { get; set; }

    public List<ContactoClienteRequest> Contactos { get; set; } = [];

    public List<DireccionClienteRequest> Direcciones { get; set; } = [];
}