using KernixERP.DTOs.Common;
using KernixERP.DTOs.Marcas.Requests;
using KernixERP.DTOs.Marcas.Responses;

namespace KernixERP.Interfaces.Inventario;

public interface IMarcaService
{
    Task<PagedResponse<MarcaResponse>> ListarAsync(
        MarcaFiltroRequest filtro,
        CancellationToken cancellationToken);

    Task<MarcaDetalleResponse?> ObtenerPorIdAsync(
        int idMarca,
        CancellationToken cancellationToken);

    Task<int> CrearAsync(
        MarcaCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken);

    Task<bool> ActualizarAsync(
        int idMarca,
        MarcaActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken);

    Task<bool> CambiarEstadoAsync(
        int idMarca,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken);
}