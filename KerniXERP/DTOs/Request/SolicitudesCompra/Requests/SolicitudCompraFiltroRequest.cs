namespace KernixERP.DTOs.SolicitudesCompra.Requests;

public sealed class SolicitudCompraFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdEmpleadoSolicita { get; set; }

    public int? IdEmpleadoAprueba { get; set; }

    public string? Prioridad { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaDesde { get; set; }

    public DateTime? FechaHasta { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}