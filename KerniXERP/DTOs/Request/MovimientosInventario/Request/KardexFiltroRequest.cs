namespace KernixERP.DTOs.MovimientosInventario.Requests;

public sealed class KardexFiltroRequest
{
    public int IdEmpresa { get; set; }

    public int IdProducto { get; set; }

    public int? IdAlmacen { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 50;
}