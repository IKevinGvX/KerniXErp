namespace KernixERP.DTOs.Sucursales.Responses;

public sealed class SucursalResponse
{
    public int IdSucursal { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Ubigeo { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }
}