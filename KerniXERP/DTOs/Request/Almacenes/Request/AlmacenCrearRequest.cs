using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Almacenes.Requests;

public sealed class AlmacenCrearRequest
{

    [Range(1, int.MaxValue,
        ErrorMessage = "La sucursal es obligatoria.")]
    public int IdSucursal { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Ubicacion { get; set; }
}