using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Clientes.Requests;

public sealed class DireccionClienteRequest
{
    [StringLength(60)]
    public string? TipoDireccion { get; set; }

    [Required]
    [StringLength(500)]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Referencia { get; set; }

    [StringLength(200)]
    public string? Distrito { get; set; }

    [StringLength(200)]
    public string? Provincia { get; set; }

    [StringLength(200)]
    public string? Departamento { get; set; }

    [StringLength(6, MinimumLength = 6)]
    public string? Ubigeo { get; set; }

    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; } = true;
}