namespace KernixERP.DTOs.Compras.DocumentosProveedor;

public sealed class DocumentoProveedorResponse
{
    public long IdDocumentoProveedor { get; set; }

    public int IdEmpresa { get; set; }

    public int IdProveedor { get; set; }

    public string Proveedor { get; set; }
        = string.Empty;

    public string CodigoTipoDocumento { get; set; }
        = string.Empty;

    public string TipoDocumento { get; set; }
        = string.Empty;

    public string Naturaleza { get; set; }
        = string.Empty;

    public long? IdDevolucionCompra { get; set; }

    public long? IdCuentaPorPagar { get; set; }

    public string NumeroDocumento { get; set; }
        = string.Empty;

    public string? NumeroDocumentoReferencia { get; set; }

    public DateTime FechaEmision { get; set; }

    public string Moneda { get; set; }
        = string.Empty;

    public decimal Total { get; set; }

    public string? Motivo { get; set; }

    public string? Observacion { get; set; }

    public string Estado { get; set; }
        = string.Empty;

    public DateTime FechaRegistro { get; set; }
}