namespace KernixERP.DTOs.Usuarios.Requests;

public sealed class UsuarioFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public bool? Estado { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}