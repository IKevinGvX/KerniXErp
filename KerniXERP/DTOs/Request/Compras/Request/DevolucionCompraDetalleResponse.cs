namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraDetalleResponse
{
    public long IdDevolucionCompra { get; set; }

    public string NumeroDevolucion { get; set; } = string.Empty;

    public int IdEmpresa { get; set; }

    public int IdProveedor { get; set; }

    public string Proveedor { get; set; } = string.Empty;

    public long? IdOrdenCompra { get; set; }

    public string? NumeroOrden { get; set; }

    public long IdRecepcionCompra { get; set; }

    public string NumeroRecepcion { get; set; } = string.Empty;

    public int IdAlmacen { get; set; }

    public string Almacen { get; set; } = string.Empty;

    public int IdEmpleado { get; set; }

    public string Empleado { get; set; } = string.Empty;

    public DateTime FechaDevolucion { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public string? NumeroDocumentoReferencia { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string? Observacion { get; set; }

    public List<DevolucionCompraDetalleItemResponse> Detalles { get; set; } = [];
}