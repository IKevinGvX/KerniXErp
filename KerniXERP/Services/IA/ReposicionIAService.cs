using KernixERP.DTOs.IA;
using KernixERP.DTOs.SolicitudesCompra.Requests;
using KernixERP.Interfaces.Compras;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class ReposicionIAService
    : IReposicionIAService
{
    public Task<IReadOnlyCollection<RecomendacionIAResponse>>
    ListarRecomendacionesAsync(
        int idUsuario,
        string? estado,
        CancellationToken cancellationToken)
    {
        if (idUsuario <= 0)
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");

        return _repository.ListarRecomendacionesAsync(
            idUsuario,
            estado,
            cancellationToken);
    }

    public Task<RecomendacionIAResponse?>
        ObtenerRecomendacionPorIdAsync(
            long idRecomendacionIA,
            int idUsuario,
            CancellationToken cancellationToken)
    {
        if (idRecomendacionIA <= 0)
            throw new ArgumentException(
                "La recomendación no es válida.");

        return _repository.ObtenerRecomendacionPorIdAsync(
            idRecomendacionIA,
            idUsuario,
            cancellationToken);
    }

    public Task DescartarRecomendacionAsync(
        long idRecomendacionIA,
        int idUsuario,
        CancellationToken cancellationToken)
    {
        if (idRecomendacionIA <= 0)
            throw new ArgumentException(
                "La recomendación no es válida.");

        return _repository.DescartarRecomendacionAsync(
            idRecomendacionIA,
            idUsuario,
            cancellationToken);
    }
    private readonly IReposicionIARepository _repository;
    private readonly ISolicitudCompraService _solicitudCompraService;

    public ReposicionIAService(
        IReposicionIARepository repository,
        ISolicitudCompraService solicitudCompraService)
    {
        _repository = repository;
        _solicitudCompraService = solicitudCompraService;
    }

    public Task<IReadOnlyCollection<
        RecomendacionReposicionResponse>>
        AnalizarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken)
    {
        ValidarAnalisis(
            idEmpresa,
            idUsuario,
            request);

        return _repository.AnalizarAsync(
            idEmpresa,
            idUsuario,
            request,
            cancellationToken);
    }

    public async Task<
        IReadOnlyCollection<ReposicionGeneradaResponse>>
        GenerarAsync(
            int idEmpresa,
            int idUsuario,
            ReposicionAnalisisRequest request,
            CancellationToken cancellationToken)
    {
        var analisis =
            await AnalizarAsync(
                idEmpresa,
                idUsuario,
                request,
                cancellationToken);

        var generadas =
            new List<ReposicionGeneradaResponse>();

        foreach (var item in analisis)
        {
            var idPrediccion =
                await _repository.GuardarPrediccionAsync(
                    idUsuario,
                    item,
                    request.DiasHistorial,
                    request.DiasProyeccion,
                    cancellationToken);

            var idRecomendacion =
                await _repository.GuardarRecomendacionAsync(
                    idUsuario,
                    item,
                    cancellationToken);

            generadas.Add(
                new ReposicionGeneradaResponse
                {
                    IdProducto = item.IdProducto,
                    Producto = item.Producto,
                    IdPrediccionIA = idPrediccion,
                    IdRecomendacionIA = idRecomendacion,
                    CantidadSugerida = item.CantidadSugerida,
                    NivelRiesgo = item.NivelRiesgo
                });
        }

        return generadas;
    }

    public async Task<SolicitudCompraDesdeIAResponse>
        GenerarSolicitudCompraAsync(
            long idRecomendacionIA,
            int idEmpresa,
            int idSucursal,
            int idEmpleado,
            int idUsuario,
            CancellationToken cancellationToken)
    {
        if (idRecomendacionIA <= 0)
        {
            throw new ArgumentException(
                "La recomendación IA no es válida.");
        }

        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa no es válida.");
        }

        if (idSucursal <= 0)
        {
            throw new ArgumentException(
                "La sucursal no es válida.");
        }

        if (idEmpleado <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado.");
        }

        if (idUsuario <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");
        }

        var recomendacion =
            await _repository.ObtenerRecomendacionAsync(
                idRecomendacionIA,
                idUsuario,
                cancellationToken);

        if (recomendacion is null)
        {
            throw new InvalidOperationException(
                "La recomendación no existe o ya fue procesada.");
        }

        if (recomendacion.IdProducto <= 0)
        {
            throw new InvalidOperationException(
                "La recomendación no contiene un producto válido.");
        }

        if (recomendacion.CantidadSugerida <= 0)
        {
            throw new InvalidOperationException(
                "La cantidad sugerida debe ser mayor a cero.");
        }

        var solicitud =
            new SolicitudCompraCrearRequest
            {
                IdEmpresa = idEmpresa,

                IdSucursal = idSucursal,

                FechaNecesaria =
                    DateTime.Now.Date.AddDays(7),

                Prioridad =
                    ConvertirPrioridad(
                        recomendacion.Prioridad),

                Motivo =
                    "Reposición recomendada por KerniX AI.",

                Observacion =
                    $"Generada desde la recomendación IA " +
                    $"#{idRecomendacionIA}.",

                Detalles =
                [
                    new DetalleSolicitudCompraCrearRequest
                    {
                        IdProducto =
                            recomendacion.IdProducto,

                        CantidadSolicitada =
                            recomendacion.CantidadSugerida,

                        Motivo =
                            "Reposición de inventario recomendada " +
                            "por KerniX AI.",

                        Observacion =
                            recomendacion.Descripcion
                    }
                ]
            };

        var resultado =
            await _solicitudCompraService.CrearAsync(
                solicitud,
                idEmpleado,
                cancellationToken);

        await _repository.ProcesarRecomendacionAsync(
            idRecomendacionIA,
            idUsuario,
            cancellationToken);

        return new SolicitudCompraDesdeIAResponse
        {
            IdRecomendacionIA =
                idRecomendacionIA,

            IdSolicitudCompra =
                resultado.IdSolicitudCompra,

            NumeroSolicitud =
                resultado.NumeroSolicitud,

            IdProducto =
                recomendacion.IdProducto,

            CantidadSolicitada =
                recomendacion.CantidadSugerida
        };
    }

    private static void ValidarAnalisis(
        int idEmpresa,
        int idUsuario,
        ReposicionAnalisisRequest request)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (idUsuario <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al usuario.");
        }

        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }

        if (request.IdAlmacen.HasValue &&
            request.IdAlmacen.Value <= 0)
        {
            throw new ArgumentException(
                "El almacén no es válido.");
        }

        if (request.DiasHistorial < 7 ||
            request.DiasHistorial > 365)
        {
            throw new ArgumentException(
                "Los días de historial deben estar entre 7 y 365.");
        }

        if (request.DiasProyeccion < 1 ||
            request.DiasProyeccion > 365)
        {
            throw new ArgumentException(
                "Los días de proyección deben estar entre 1 y 365.");
        }
    }

    private static string ConvertirPrioridad(
        string? prioridad)
    {
        return prioridad?
            .Trim()
            .ToLowerInvariant() switch
        {
            "urgente" => "alta",
            "alta" => "alta",
            "media" => "media",
            "baja" => "baja",
            _ => "media"
        };
    }
}