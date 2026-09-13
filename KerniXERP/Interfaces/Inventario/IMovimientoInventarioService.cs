using KernixERP.DTOs.Common;
using KernixERP.DTOs.MovimientosInventario.Requests;
using KernixERP.DTOs.MovimientosInventario.Responses;

namespace KernixERP.Interfaces.Inventario;

public interface IMovimientoInventarioService
{
    Task<PagedResponse<MovimientoInventarioResponse>> ListarAsync(
        MovimientoInventarioFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<MovimientoInventarioDetalleResponse?> ObtenerPorIdAsync(
        long idMovimientoInventario,
        CancellationToken cancellationToken);

    Task<long> RegistrarAsync(
        MovimientoInventarioCrearRequest request,
        int idEmpleado,
        CancellationToken cancellationToken);

    Task<KardexResponse?> ObtenerKardexAsync(
        KardexFiltroRequest filtro,
        CancellationToken cancellationToken);
}