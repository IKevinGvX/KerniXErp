namespace KernixERP.DTOs.CuentasPorPagar.Responses;

public sealed class CuentaPorPagarDetalleResponse
{
    public long IdCuentaPorPagar { get; set; }

    public int IdEmpresa { get; set; }

    public int IdProveedor { get; set; }

    public long? IdOrdenCompra { get; set; }

    public string NumeroDocumento { get; set; } =
        string.Empty;

    public string TipoDocumento { get; set; } =
        string.Empty;

    public DateTime FechaEmision { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public decimal MontoTotal { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal SaldoPendiente { get; set; }

    public string Moneda { get; set; } =
        string.Empty;

    public string Estado { get; set; } =
        string.Empty;

    public List<PagoProveedorResponse> Pagos { get; set; } =
        [];
}