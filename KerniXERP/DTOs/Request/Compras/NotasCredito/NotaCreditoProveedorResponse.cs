namespace KernixERP.DTOs.Compras.NotasCredito;

public sealed class NotaCreditoProveedorResponse
{
    public long IdNotaCreditoProveedor { get; set; }

    public int IdProveedor { get; set; }

    public string Proveedor { get; set; } = string.Empty;

    public long? IdDevolucionCompra { get; set; }

    public string? NumeroDevolucion { get; set; }

    public long IdCuentaPorPagar { get; set; }

    public string NumeroNota { get; set; } = string.Empty;

    public string? NumeroDocumentoReferencia { get; set; }

    public DateTime FechaEmision { get; set; }

    public string Moneda { get; set; } = string.Empty;

    public decimal Monto { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }
}