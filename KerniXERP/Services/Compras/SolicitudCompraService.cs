using KernixERP.DTOs.Common;
using KernixERP.DTOs.SolicitudesCompra.Requests;
using KernixERP.DTOs.SolicitudesCompra.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class SolicitudCompraService
    : ISolicitudCompraService
{
    private static readonly HashSet<string> PrioridadesPermitidas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "baja",
            "media",
            "alta",
            "urgente"
        };

    private static readonly HashSet<string> EstadosPermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "pendiente",
            "aprobada",
            "rechazada",
            "anulada"
        };

    private readonly ISolicitudCompraRepository _repository;

    public SolicitudCompraService(
        ISolicitudCompraRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<SolicitudCompraResponse>> ListarAsync(
        SolicitudCompraFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (filtro.IdEmpresa is null ||
            filtro.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        filtro.Pagina =
            Math.Max(
                filtro.Pagina,
                1);

        filtro.TamanoPagina =
            Math.Clamp(
                filtro.TamanoPagina,
                1,
                100);

        filtro.Prioridad =
            NormalizarTexto(filtro.Prioridad)?
                .ToLowerInvariant();

        filtro.Estado =
            NormalizarTexto(filtro.Estado)?
                .ToLowerInvariant();

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        if (filtro.Prioridad is not null &&
            !PrioridadesPermitidas.Contains(
                filtro.Prioridad))
        {
            throw new ArgumentException(
                "La prioridad debe ser baja, media, alta o urgente.");
        }

        if (filtro.Estado is not null &&
            !EstadosPermitidos.Contains(
                filtro.Estado))
        {
            throw new ArgumentException(
                "El estado indicado no es válido.");
        }

        if (filtro.FechaDesde.HasValue &&
            filtro.FechaHasta.HasValue &&
            filtro.FechaHasta.Value.Date <
            filtro.FechaDesde.Value.Date)
        {
            throw new ArgumentException(
                "La fecha final no puede ser anterior a la fecha inicial.");
        }

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<SolicitudCompraDetalleResponse?> ObtenerPorIdAsync(
        long idSolicitudCompra,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idSolicitudCompra <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult<
                SolicitudCompraDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idSolicitudCompra,
            idEmpresa,
            cancellationToken);
    }

    public Task<(long IdSolicitudCompra, string NumeroSolicitud)> CrearAsync(
        SolicitudCompraCrearRequest request,
        int idEmpleadoSolicita,
        CancellationToken cancellationToken)
    {
        if (request.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (request.IdSucursal <= 0)
        {
            throw new ArgumentException(
                "La sucursal es obligatoria.");
        }

        if (idEmpleadoSolicita <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado solicitante.");
        }

        NormalizarCreacion(request);

        if (request.Detalles is null ||
            request.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "La solicitud debe contener al menos un producto.");
        }

        var duplicados =
            request.Detalles
                .GroupBy(x => x.IdProducto)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

        if (duplicados.Count > 0)
        {
            throw new ArgumentException(
                "No se puede repetir un producto en la solicitud.");
        }

        foreach (var detalle in request.Detalles)
        {
            if (detalle.IdProducto <= 0)
            {
                throw new ArgumentException(
                    "Existe un producto inválido en la solicitud.");
            }

            if (detalle.CantidadSolicitada <= 0)
            {
                throw new ArgumentException(
                    "Las cantidades solicitadas deben ser mayores que cero.");
            }
        }

        if (request.FechaNecesaria.HasValue &&
            request.FechaNecesaria.Value.Date <
            DateTime.Today)
        {
            throw new ArgumentException(
                "La fecha necesaria no puede ser anterior a la fecha actual.");
        }

        if (!PrioridadesPermitidas.Contains(
                request.Prioridad ?? "media"))
        {
            throw new ArgumentException(
                "La prioridad debe ser baja, media, alta o urgente.");
        }

        return _repository.CrearAsync(
            request,
            idEmpleadoSolicita,
            cancellationToken);
    }

    public async Task<bool> AprobarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        SolicitudCompraAprobarRequest request,
        CancellationToken cancellationToken)
    {
        if (idSolicitudCompra <= 0 ||
            idEmpresa <= 0)
        {
            return false;
        }

        if (idEmpleadoAprueba <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado aprobador.");
        }

        var solicitud =
            await _repository.ObtenerPorIdAsync(
                idSolicitudCompra,
                idEmpresa,
                cancellationToken);

        if (solicitud is null)
        {
            return false;
        }

        if (!solicitud.Estado.Equals(
                "pendiente",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Solo se pueden aprobar solicitudes pendientes.");
        }

        if (solicitud.IdEmpleadoSolicita ==
            idEmpleadoAprueba)
        {
            throw new InvalidOperationException(
                "El empleado solicitante no puede aprobar su propia solicitud.");
        }

        request.Observacion =
            NormalizarTexto(
                request.Observacion);

        if (request.Detalles is null ||
            request.Detalles.Count == 0)
        {
            throw new ArgumentException(
                "Debe indicar las cantidades aprobadas.");
        }

        if (request.Detalles.Count !=
            solicitud.Detalles.Count)
        {
            throw new ArgumentException(
                "Debe aprobar todos los productos de la solicitud.");
        }

        var duplicados =
            request.Detalles
                .GroupBy(x =>
                    x.IdDetalleSolicitud)
                .Any(x =>
                    x.Count() > 1);

        if (duplicados)
        {
            throw new ArgumentException(
                "Existen detalles repetidos en la aprobación.");
        }

        foreach (var detalle in request.Detalles)
        {
            var original =
                solicitud.Detalles
                    .FirstOrDefault(x =>
                        x.IdDetalleSolicitud ==
                        detalle.IdDetalleSolicitud);

            if (original is null)
            {
                throw new ArgumentException(
                    "Uno de los detalles no pertenece a la solicitud.");
            }

            if (detalle.CantidadAprobada <= 0)
            {
                throw new ArgumentException(
                    "La cantidad aprobada debe ser mayor que cero.");
            }

            if (detalle.CantidadAprobada >
                original.CantidadSolicitada)
            {
                throw new ArgumentException(
                    $"La cantidad aprobada para {original.Producto} supera la cantidad solicitada.");
            }

            detalle.Observacion =
                NormalizarTexto(
                    detalle.Observacion);
        }

        return await _repository.AprobarAsync(
            idSolicitudCompra,
            idEmpresa,
            idEmpleadoAprueba,
            request,
            cancellationToken);
    }

    public async Task<bool> RechazarAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleadoAprueba,
        SolicitudCompraProcesarRequest request,
        CancellationToken cancellationToken)
    {
        request.Observacion =
            NormalizarTexto(request.Observacion)
            ?? throw new ArgumentException(
                "El motivo del rechazo es obligatorio.");

        var solicitud =
            await _repository.ObtenerPorIdAsync(
                idSolicitudCompra,
                idEmpresa,
                cancellationToken);

        if (solicitud is null)
        {
            return false;
        }

        if (!solicitud.Estado.Equals(
                "pendiente",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Solo se pueden rechazar solicitudes pendientes.");
        }

        if (solicitud.IdEmpleadoSolicita ==
            idEmpleadoAprueba)
        {
            throw new InvalidOperationException(
                "El empleado solicitante no puede rechazar su propia solicitud.");
        }

        return await _repository.RechazarAsync(
            idSolicitudCompra,
            idEmpresa,
            idEmpleadoAprueba,
            request.Observacion,
            cancellationToken);
    }

    public async Task<bool> AnularAsync(
        long idSolicitudCompra,
        int idEmpresa,
        int idEmpleado,
        SolicitudCompraProcesarRequest request,
        CancellationToken cancellationToken)
    {
        request.Observacion =
            NormalizarTexto(request.Observacion)
            ?? throw new ArgumentException(
                "El motivo de anulación es obligatorio.");

        var solicitud =
            await _repository.ObtenerPorIdAsync(
                idSolicitudCompra,
                idEmpresa,
                cancellationToken);

        if (solicitud is null)
        {
            return false;
        }

        if (!solicitud.Estado.Equals(
                "pendiente",
                StringComparison.OrdinalIgnoreCase) &&
            !solicitud.Estado.Equals(
                "aprobada",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La solicitud no puede anularse en su estado actual.");
        }

        return await _repository.AnularAsync(
            idSolicitudCompra,
            idEmpresa,
            idEmpleado,
            request.Observacion,
            cancellationToken);
    }

    private static void NormalizarCreacion(
        SolicitudCompraCrearRequest request)
    {
        request.Prioridad =
            NormalizarTexto(request.Prioridad)?
                .ToLowerInvariant()
            ?? "media";

        request.Motivo =
            NormalizarTexto(request.Motivo);

        request.Observacion =
            NormalizarTexto(request.Observacion);

        foreach (var detalle in request.Detalles ?? [])
        {
            detalle.Motivo =
                NormalizarTexto(
                    detalle.Motivo);

            detalle.Observacion =
                NormalizarTexto(
                    detalle.Observacion);
        }
    }

    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}