using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Marcas.Requests;

public sealed class MarcaActualizarRequest
{
    [Range(1, int.MaxValue,
        ErrorMessage = "La empresa es obligatoria.")]
    public int IdEmpresa { get; set; }

    [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    public bool Estado { get; set; }
}