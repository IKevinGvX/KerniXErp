namespace KernixERP.DTOs.CuentasPorPagar.Responses;

public sealed class PagoProveedorResponse
{
    public long IdPagoProveedor { get; set; }

    public long IdCuentaPorPagar { get; set; }

    public int IdEmpleado { get; set; }

    public string Empleado { get; set; } =
        string.Empty;

    public string MetodoPago { get; set; } =
        string.Empty;

    public string Moneda { get; set; } =
        string.Empty;

    public decimal Monto { get; set; }

    public decimal? TipoCambio { get; set; }

    public string? NumeroOperacion { get; set; }

    public DateTime FechaPago { get; set; }

    public string Estado { get; set; } =
        string.Empty;

    public string? Observacion { get; set; }
}