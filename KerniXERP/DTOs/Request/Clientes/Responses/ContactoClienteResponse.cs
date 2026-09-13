namespace KernixERP.DTOs.Clientes.Responses;

public sealed class ContactoClienteResponse
{
    public int IdContactoCliente { get; set; }

    public int IdCliente { get; set; }

    public string Nombres { get; set; } = string.Empty;

    public string? Cargo { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool EsPrincipal { get; set; }

    public bool Estado { get; set; }
}