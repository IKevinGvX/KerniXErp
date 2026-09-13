namespace KernixERP.DTOs.Empleados.Requests;

public sealed class EmpleadoFiltroRequest
{
    public int? IdEmpresa { get; set; }

    public int? IdSucursal { get; set; }

    public bool? Estado { get; set; }

    public string? Buscar { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanoPagina { get; set; } = 20;
}