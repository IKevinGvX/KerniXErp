namespace KernixERP.DTOs.Sucursales.Requests;

public sealed class SucursalFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public bool? Estado { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}