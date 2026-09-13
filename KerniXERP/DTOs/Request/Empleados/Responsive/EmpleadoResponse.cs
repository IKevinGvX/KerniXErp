namespace KernixERP.DTOs.Empleados.Responses;

public sealed class EmpleadoResponse
{
    public int IdEmpleado { get; set; }

    public int IdEmpresa { get; set; }

    public int IdSucursal { get; set; }

    public string Empresa { get; set; } = string.Empty;

    public string Sucursal { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string ApellidoPaterno { get; set; } = string.Empty;

    public string? ApellidoMaterno { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Cargo { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaCese { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }
}