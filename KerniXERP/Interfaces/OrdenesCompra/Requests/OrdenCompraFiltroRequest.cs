namespace KernixERP.DTOs.OrdenesCompra.Requests;

public sealed class OrdenCompraFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdProveedor { get; set; }

    public int? IdEmpleado { get; set; }

    public string? Estado { get; set; }

    public string? Moneda { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}