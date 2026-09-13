namespace KernixERP.DTOs.Proveedores.Responses;

public sealed class ProveedorDetalleResponse
{
    public int IdProveedor { get; set; }

    public int IdEmpresa { get; set; }

    public string TipoDocumento { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string RazonSocial { get; set; } = string.Empty;

    public string? NombreComercial { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public List<ContactoProveedorResponse> Contactos { get; set; } = [];

    public List<DireccionProveedorResponse> Direcciones { get; set; } = [];
}