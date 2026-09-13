namespace KernixERP.DTOs.UnidadesMedida.Responses;

public sealed class UnidadMedidaResponse
{
    public int IdUnidadMedida { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Abreviatura { get; set; } = string.Empty;

    public bool Estado { get; set; }

    public int TotalProductos { get; set; }
}