using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Productos.Requests;

public sealed class ProductoActualizarRequest
{
    [Range(1, int.MaxValue)]
    public int IdEmpresa { get; set; }

    [Range(1, int.MaxValue)]
    public int IdCategoriaProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdMarca { get; set; }

    [Range(1, int.MaxValue)]
    public int IdUnidadMedida { get; set; }

    [Required]
    [StringLength(100)]
    public string Codigo { get; set; } = string.Empty;

    [StringLength(100)]
    public string? CodigoBarras { get; set; }

    [Required]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(0, 999999999999.99)]
    public decimal PrecioCompra { get; set; }

    [Range(0, 999999999999.99)]
    public decimal PrecioVenta { get; set; }

    [Range(0, 999999999999.9999)]
    public decimal StockMinimo { get; set; }

    [Range(0, 999999999999.9999)]
    public decimal? StockMaximo { get; set; }

    public bool ControlaStock { get; set; }

    public bool Estado { get; set; }
}