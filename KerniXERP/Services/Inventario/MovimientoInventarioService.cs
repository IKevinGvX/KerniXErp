using KernixERP.DTOs.Common;
using KernixERP.DTOs.MovimientosInventario.Requests;
using KernixERP.DTOs.MovimientosInventario.Responses;
using KernixERP.Interfaces.Inventario;

namespace KernixERP.Services.Inventario;

public sealed class MovimientoInventarioService
    : IMovimientoInventarioService
{
    /*
     * Estos tipos pueden registrarse manualmente desde este endpoint:
     *
     * 1 = entrada
     * 2 = salida
     * 3 = ajuste positivo
     * 4 = ajuste negativo
     *
     * Los tipos de transferencia 5 y 6 serán generados
     * exclusivamente por el módulo de transferencias.
     */
    private static readonly HashSet<int> TiposMovimientoManuales =
    [
        1,
        2,
        3,
        4
    ];

    private static readonly HashSet<string> NaturalezasPermitidas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "ingreso",
            "egreso"
        };

    private readonly IMovimientoInventarioRepository _repository;

    public MovimientoInventarioService(
        IMovimientoInventarioRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<MovimientoInventarioResponse>> ListarAsync(
        MovimientoInventarioFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(
            filtro.Pagina,
            1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        filtro.Naturaleza =
            NormalizarTexto(filtro.Naturaleza)?
                .ToLowerInvariant();

        ValidarIdOpcional(
            filtro.IdEmpresa,
            "empresa");

        ValidarIdOpcional(
            filtro.IdSucursal,
            "sucursal");

        ValidarIdOpcional(
            filtro.IdAlmacen,
            "almacén");

        ValidarIdOpcional(
            filtro.IdProducto,
            "producto");

        ValidarIdOpcional(
            filtro.IdTipoMovimiento,
            "tipo de movimiento");

        ValidarIdOpcional(
            filtro.IdEmpleado,
            "empleado");

        if (filtro.Naturaleza is not null &&
            !NaturalezasPermitidas.Contains(
                filtro.Naturaleza))
        {
            throw new ArgumentException(
                "La naturaleza debe ser ingreso o egreso.");
        }

        ValidarRangoFechas(
            filtro.FechaDesde,
            filtro.FechaHasta);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<MovimientoInventarioDetalleResponse?> ObtenerPorIdAsync(
        long idMovimientoInventario,
        CancellationToken cancellationToken)
    {
        if (idMovimientoInventario <= 0)
        {
            return Task.FromResult<
                MovimientoInventarioDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idMovimientoInventario,
            cancellationToken);
    }

    public Task<long> RegistrarAsync(
        MovimientoInventarioCrearRequest request,
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        if (idEmpleado <= 0)
        {
            throw new UnauthorizedAccessException(
                "No se pudo identificar al empleado autenticado.");
        }

        if (request.IdAlmacen <= 0)
        {
            throw new ArgumentException(
                "El almacén es obligatorio.");
        }

        if (request.IdProducto <= 0)
        {
            throw new ArgumentException(
                "El producto es obligatorio.");
        }

        if (!TiposMovimientoManuales.Contains(
                request.IdTipoMovimiento))
        {
            throw new ArgumentException(
                "El tipo de movimiento no está permitido para registro manual. " +
                "Use únicamente entrada, salida, ajuste positivo o ajuste negativo.");
        }

        if (request.Cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero.");
        }

        Normalizar(request);

        ValidarMotivoPorTipo(request);

        return _repository.RegistrarAsync(
            request,
            idEmpleado,
            cancellationToken);
    }

    public Task<KardexResponse?> ObtenerKardexAsync(
        KardexFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (filtro.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (filtro.IdProducto <= 0)
        {
            throw new ArgumentException(
                "El producto es obligatorio.");
        }

        ValidarIdOpcional(
            filtro.IdAlmacen,
            "almacén");

        ValidarRangoFechas(
            filtro.FechaDesde,
            filtro.FechaHasta);

        filtro.Pagina = Math.Max(
            filtro.Pagina,
            1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            200);

        return _repository.ObtenerKardexAsync(
            filtro,
            cancellationToken);
    }

    private static void Normalizar(
        MovimientoInventarioCrearRequest request)
    {
        request.Referencia =
            NormalizarTexto(request.Referencia);

        request.Motivo =
            NormalizarTexto(request.Motivo);
    }

    private static void ValidarMotivoPorTipo(
        MovimientoInventarioCrearRequest request)
    {
        /*
         * En ajustes, el motivo debe ser obligatorio
         * para mantener trazabilidad.
         */
        var esAjuste =
            request.IdTipoMovimiento is 3 or 4;

        if (esAjuste &&
            string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ArgumentException(
                "El motivo es obligatorio para los ajustes de inventario.");
        }
    }

    private static void ValidarRangoFechas(
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        if (fechaDesde.HasValue &&
            fechaHasta.HasValue &&
            fechaHasta.Value.Date <
            fechaDesde.Value.Date)
        {
            throw new ArgumentException(
                "La fecha final no puede ser anterior a la fecha inicial.");
        }
    }

    private static void ValidarIdOpcional(
        int? id,
        string nombre)
    {
        if (id.HasValue &&
            id.Value <= 0)
        {
            throw new ArgumentException(
                $"El identificador de {nombre} no es válido.");
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