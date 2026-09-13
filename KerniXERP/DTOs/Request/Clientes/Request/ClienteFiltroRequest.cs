namespace KernixERP.DTOs.Clientes.Requests;

public sealed class ClienteFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public string? TipoCliente { get; set; }

    public string? TipoDocumento { get; set; }

    public bool? Estado { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}