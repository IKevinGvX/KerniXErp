namespace KernixERP.DTOs.Clientes.Responses;

public sealed class ClienteIndicadoresResponse
{
    public int TotalVentas { get; set; }

    public decimal TotalComprado { get; set; }

    public DateTime? FechaUltimaCompra { get; set; }

    public int TotalContactos { get; set; }

    public int TotalDirecciones { get; set; }
}