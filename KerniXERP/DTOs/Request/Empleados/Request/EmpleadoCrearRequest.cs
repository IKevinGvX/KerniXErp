using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Empleados.Requests;

public sealed class EmpleadoCrearRequest
{
    [Range(1, int.MaxValue,
        ErrorMessage = "La empresa es obligatoria.")]
    public int IdEmpresa { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "La sucursal es obligatoria.")]
    public int IdSucursal { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
    [StringLength(100)]
    public string ApellidoPaterno { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ApellidoMaterno { get; set; }

    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    [StringLength(20)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string? Correo { get; set; }

    [StringLength(100)]
    public string? Cargo { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaCese { get; set; }
}