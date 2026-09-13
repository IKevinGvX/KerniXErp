namespace KernixERP.DTOs.Empresas.Responses;

public sealed class EmpresaDetalleResponse
{
    public int IdEmpresa { get; set; }

    public string RazonSocial { get; set; } = string.Empty;

    public string? NombreComercial { get; set; }

    public string Ruc { get; set; } = string.Empty;

    public string? DireccionFiscal { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? SitioWeb { get; set; }

    public string? LogoUrl { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public int TotalSucursales { get; set; }

    public int TotalEmpleados { get; set; }

    public int TotalAlmacenes { get; set; }
}