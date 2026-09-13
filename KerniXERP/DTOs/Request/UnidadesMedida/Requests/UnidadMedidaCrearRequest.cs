using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.UnidadesMedida.Requests;

public sealed class UnidadMedidaCrearRequest
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La abreviatura es obligatoria.")]
    [StringLength(20)]
    public string Abreviatura { get; set; } = string.Empty;
}