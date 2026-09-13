namespace KernixERP.DTOs.Proveedores.Responses;

public sealed class DireccionProveedorResponse
{
    public int IdDireccionProveedor { get; set; }

    public int IdProveedor { get; set; }

    public string? TipoDireccion { get; set; }

    public string Direccion { get; set; } = string.Empty;

    public string? Referencia { get; set; }

    public string? Distrito { get; set; }

    public string? Provincia { get; set; }

    public string? Departamento { get; set; }

    public string? Ubigeo { get; set; }

    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; }
}