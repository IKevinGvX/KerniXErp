namespace KernixERP.DTOs.OrdenesCompra.Responses;

public sealed class OrdenCompraResponse
{
    public long IdOrdenCompra { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public int IdSucursal { get; set; }

    public string Sucursal { get; set; } = string.Empty;

    public int IdProveedor { get; set; }

    public string Proveedor { get; set; } = string.Empty;

    public string NumeroDocumentoProveedor { get; set; } = string.Empty;

    public int IdEmpleado { get; set; }

    public string Empleado { get; set; } = string.Empty;

    public long? IdSolicitudCompra { get; set; }

    public string? NumeroSolicitud { get; set; }

    public string NumeroOrden { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }

    public string Moneda { get; set; } = string.Empty;

    public decimal TipoCambio { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public string? CondicionPago { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public int TotalProductos { get; set; }

    public decimal CantidadOrdenada { get; set; }

    public decimal CantidadRecibida { get; set; }
}