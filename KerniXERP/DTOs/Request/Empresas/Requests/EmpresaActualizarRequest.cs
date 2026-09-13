using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Empresas.Requests;

public sealed class EmpresaActualizarRequest
{
    [Required]
    [RegularExpression(@"^\d{11}$",
        ErrorMessage = "El RUC debe contener exactamente 11 números.")]
    public string Ruc { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string RazonSocial { get; set; } = string.Empty;

    [StringLength(150)]
    public string? NombreComercial { get; set; }

    [StringLength(250)]
    public string? DireccionFiscal { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(250)]
    public string? SitioWeb { get; set; }

    [StringLength(500)]
    public string? LogoUrl { get; set; }

    public bool Estado { get; set; }
}