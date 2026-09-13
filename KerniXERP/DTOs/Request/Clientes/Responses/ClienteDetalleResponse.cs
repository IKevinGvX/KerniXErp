namespace KernixERP.DTOs.Clientes.Responses;

public sealed class ClienteDetalleResponse
{
    public int IdCliente { get; set; }

    public int IdEmpresa { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string TipoCliente { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string? RazonSocial { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public List<ContactoClienteResponse> Contactos { get; set; } = [];

    public List<DireccionClienteResponse> Direcciones { get; set; } = [];

    public ClienteIndicadoresResponse Indicadores { get; set; } = new();
}