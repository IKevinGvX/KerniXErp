namespace KernixERP.DTOs.Proveedores.Responses;

public sealed class ContactoProveedorResponse
{
    public int IdContactoProveedor { get; set; }

    public int IdProveedor { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string? Cargo { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; }
}