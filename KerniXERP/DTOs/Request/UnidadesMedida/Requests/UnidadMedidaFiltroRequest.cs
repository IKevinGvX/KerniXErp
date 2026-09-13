namespace KernixERP.DTOs.UnidadesMedida.Requests;

public sealed class UnidadMedidaFiltroRequest
{
    public bool? Estado { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}