namespace KernixERP.DTOs.Almacenes.Responses;

public sealed class AlmacenResponse
{
    public int IdAlmacen { get; set; }

    public int IdSucursal { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? Ubicacion { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }
}