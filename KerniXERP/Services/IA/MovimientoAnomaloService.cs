using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class MovimientoAnomaloService
    : IMovimientoAnomaloService
{
    private readonly IMovimientoAnomaloRepository _repository;

    public MovimientoAnomaloService(
        IMovimientoAnomaloRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<MovimientoAnomaloResponse>>
        DetectarAsync(
            int idEmpresa,
            MovimientoAnomaloRequest request,
            CancellationToken cancellationToken)
    {
        ValidarRequest(
            idEmpresa,
            request);

        return _repository.DetectarAsync(
            idEmpresa,
            request,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<AnomaliaGeneradaResponse>>
        GenerarAsync(
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa no es válida.");
        }

        var request =
            new MovimientoAnomaloRequest
            {
                IdAlmacen = null,
                IdProducto = null,
                DiasHistorial = 30,
                SoloAnomalias = true
            };

        var movimientos =
            await _repository.DetectarAsync(
                idEmpresa,
                request,
                cancellationToken);

        var resultado =
            new List<AnomaliaGeneradaResponse>();

        foreach (var movimiento in movimientos)
        {
            if (!movimiento.EsAnomalo)
                continue;

            if (movimiento.NivelRiesgo != "ALTO" &&
                movimiento.NivelRiesgo != "CRITICO")
            {
                continue;
            }

            var idAnomalia =
                await _repository.RegistrarAnomaliaAsync(
                    1,
                    movimiento,
                    cancellationToken);

            resultado.Add(
                new AnomaliaGeneradaResponse
                {
                    IdAnomalia =
                        idAnomalia,

                    IdMovimientoInventario =
                        movimiento.IdMovimientoInventario,

                    IdProducto =
                        movimiento.IdProducto,

                    Producto =
                        movimiento.Producto,

                    Cantidad =
                        movimiento.Cantidad,

                    UmbralAnomalia =
                        movimiento.UmbralAnomalia,

                    NivelRiesgo =
                        movimiento.NivelRiesgo,

                    Estado =
                        "detectada"
                });
        }

        return resultado;
    }

    private static void ValidarRequest(
        int idEmpresa,
        MovimientoAnomaloRequest request)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa no es válida.");
        }

        ArgumentNullException.ThrowIfNull(request);

        if (request.IdAlmacen.HasValue &&
            request.IdAlmacen.Value <= 0)
        {
            throw new ArgumentException(
                "El almacén no es válido.");
        }

        if (request.IdProducto.HasValue &&
            request.IdProducto.Value <= 0)
        {
            throw new ArgumentException(
                "El producto no es válido.");
        }

        if (request.DiasHistorial < 7 ||
            request.DiasHistorial > 365)
        {
            throw new ArgumentException(
                "Los días de historial deben estar entre 7 y 365.");
        }
    }
    public Task<IReadOnlyCollection<AnomaliaRegistradaResponse>>
    ListarAnomaliasAsync(
        int idEmpresa,
        AnomaliaFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        ArgumentNullException.ThrowIfNull(filtro);

        return _repository.ListarAnomaliasAsync(
            idEmpresa,
            filtro,
            cancellationToken);
    }

    public Task<AnomaliaDetalleResponse?>
        ObtenerAnomaliaAsync(
            long idAnomalia,
            int idEmpresa,
            CancellationToken cancellationToken)
    {
        if (idAnomalia <= 0)
            throw new ArgumentException(
                "La anomalía no es válida.");

        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        return _repository.ObtenerAnomaliaAsync(
            idAnomalia,
            idEmpresa,
            cancellationToken);
    }

    public Task<bool> AtenderAnomaliaAsync(
        long idAnomalia,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idAnomalia <= 0)
            throw new ArgumentException(
                "La anomalía no es válida.");

        if (idEmpresa <= 0)
            throw new ArgumentException(
                "La empresa no es válida.");

        return _repository.AtenderAnomaliaAsync(
            idAnomalia,
            idEmpresa,
            cancellationToken);
    }
}