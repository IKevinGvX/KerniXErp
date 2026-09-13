using KernixERP.DTOs.Common;

namespace KernixERP.DTOs.MovimientosInventario.Responses;

public sealed class KardexResponse
{
    public KardexCabeceraResponse Producto { get; set; } = new();

    public PagedResponse<KardexMovimientoResponse> Movimientos { get; set; } =
        new();
}