namespace KernixERP.DTOs.Compras.Devoluciones;

public sealed class DevolucionCompraCrearResponse
{
    public long IdDevolucionCompra { get; set; }

    public string NumeroDevolucion { get; set; } = string.Empty;

    public long IdRecepcionCompra { get; set; }

    public long? IdOrdenCompra { get; set; }

    public int IdProveedor { get; set; }
}