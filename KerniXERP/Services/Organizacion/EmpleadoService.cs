using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empleados.Requests;
using KernixERP.DTOs.Empleados.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Services.Organizacion;

public sealed class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _repository;

    public EmpleadoService(
        IEmpleadoRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<EmpleadoResponse>> ListarAsync(
        EmpleadoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(filtro.Pagina, 1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        filtro.Buscar = NormalizarTexto(filtro.Buscar);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<EmpleadoDetalleResponse?> ObtenerPorIdAsync(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        if (idEmpleado <= 0)
        {
            return Task.FromResult<
                EmpleadoDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idEmpleado,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        EmpleadoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        Normalizar(request);
        ValidarFechas(
            request.FechaIngreso,
            request.FechaCese);

        return _repository.CrearAsync(
            request,
            idUsuarioRegistro,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idEmpleado,
        EmpleadoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idEmpleado <= 0)
        {
            return Task.FromResult(false);
        }

        Normalizar(request);
        ValidarFechas(
            request.FechaIngreso,
            request.FechaCese);

        return _repository.ActualizarAsync(
            idEmpleado,
            request,
            idUsuarioModifica,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idEmpleado,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        if (idEmpleado <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idEmpleado,
            estado,
            idUsuarioModifica,
            cancellationToken);
    }

    private static void Normalizar(
        EmpleadoCrearRequest request)
    {
        request.Nombres =
            request.Nombres.Trim();

        request.ApellidoPaterno =
            request.ApellidoPaterno.Trim();

        request.ApellidoMaterno =
            NormalizarTexto(
                request.ApellidoMaterno);

        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToUpperInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.Telefono =
            NormalizarTexto(request.Telefono);

        request.Correo =
            NormalizarCorreo(request.Correo);

        request.Cargo =
            NormalizarTexto(request.Cargo);
    }

    private static void Normalizar(
        EmpleadoActualizarRequest request)
    {
        request.Nombres =
            request.Nombres.Trim();

        request.ApellidoPaterno =
            request.ApellidoPaterno.Trim();

        request.ApellidoMaterno =
            NormalizarTexto(
                request.ApellidoMaterno);

        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToUpperInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.Telefono =
            NormalizarTexto(request.Telefono);

        request.Correo =
            NormalizarCorreo(request.Correo);

        request.Cargo =
            NormalizarTexto(request.Cargo);
    }
    private static void ValidarFechas(
        DateTime fechaIngreso,
        DateTime? fechaCese)
    {
        if (fechaCese.HasValue &&
            fechaCese.Value.Date < fechaIngreso.Date)
        {
            throw new ArgumentException(
                "La fecha de cese no puede ser anterior a la fecha de ingreso.");
        }
    }
    private static string? NormalizarTexto(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string? NormalizarCorreo(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }
}