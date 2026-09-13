using KernixERP.DTOs.Common;
using KernixERP.DTOs.UnidadesMedida.Requests;
using KernixERP.DTOs.UnidadesMedida.Responses;

namespace KernixERP.Interfaces.Inventario;

public interface IUnidadMedidaRepository
{
    Task<PagedResponse<UnidadMedidaResponse>> ListarAsync(
        UnidadMedidaFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<UnidadMedidaDetalleResponse?> ObtenerPorIdAsync(
        int idUnidadMedida,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        UnidadMedidaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idUnidadMedida,
        UnidadMedidaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idUnidadMedida,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}