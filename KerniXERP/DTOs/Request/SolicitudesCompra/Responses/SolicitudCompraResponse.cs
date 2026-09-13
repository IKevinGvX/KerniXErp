namespace KernixERP.DTOs.SolicitudesCompra.Responses;

public sealed class SolicitudCompraResponse
{
    public long IdSolicitudCompra { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string CodigoSucursal { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public int IdEmpleadoSolicita { get; set; }

    public string EmpleadoSolicita { get; set; } = string.Empty;

    public int? IdEmpleadoAprueba { get; set; }

    public string? EmpleadoAprueba { get; set; }

    public string NumeroSolicitud { get; set; } = string.Empty;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaNecesaria { get; set; }

    public string? Prioridad { get; set; }

    public string? Motivo { get; set; }

    public string Estado { get; set; } = string.Empty;

    public DateTime? FechaAprobacion { get; set; }

    public string? Observacion { get; set; }

    public int TotalProductos { get; set; }

    public decimal TotalCantidadSolicitada { get; set; }

    public decimal TotalCantidadAprobada { get; set; }
}