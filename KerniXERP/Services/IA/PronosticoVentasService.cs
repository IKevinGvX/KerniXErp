using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;
using KernixERP.Models.IA;

namespace KernixERP.Services.IA;

public sealed class PronosticoVentasService
    : IPronosticoVentasService
{
    private readonly IPronosticoVentasRepository _repository;

    public PronosticoVentasService(
        IPronosticoVentasRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<PronosticoVentaResponse>> AnalizarAsync(
        int idEmpresa,
        PronosticoVentaRequest request,
        CancellationToken cancellationToken)
    {
        ValidarRequest(idEmpresa, request);

        return _repository.AnalizarAsync(
            idEmpresa,
            request,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<PronosticoVentaGeneradoResponse>> GenerarAsync(
        int idEmpresa,
        int idUsuario,
        PronosticoVentaRequest request,
        CancellationToken cancellationToken)
    {
        ValidarRequest(idEmpresa, request);

        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido.");
        }

        var pronosticos =
            await _repository.AnalizarAsync(
                idEmpresa,
                request,
                cancellationToken);

        var resultado =
            new List<PronosticoVentaGeneradoResponse>();

        foreach (var pronostico in pronosticos)
        {
            var nivelConfianza =
                CalcularNivelConfianza(
                    pronostico.NumeroVentas);

            var idPrediccion =
                await _repository.GuardarPrediccionAsync(
                    1,
                    idUsuario,
                    pronostico,
                    cancellationToken);

            resultado.Add(
                new PronosticoVentaGeneradoResponse
                {
                    IdPrediccionIA = idPrediccion,

                    IdProducto =
                        pronostico.IdProducto,

                    Producto =
                        pronostico.Producto,

                    CantidadPronosticada =
                        pronostico.CantidadPronosticada,

                    Tendencia =
                        pronostico.Tendencia,

                    NivelConfianza =
                        nivelConfianza,

                    DiasProyeccion =
                        pronostico.DiasProyeccion
                });
        }

        return resultado;
    }

    public Task<IReadOnlyCollection<PrediccionIA>>
        ListarPrediccionesAsync(
            int idUsuario,
            CancellationToken cancellationToken)
    {
        if (idUsuario <= 0)
        {
            throw new ArgumentException(
                "El usuario no es válido.");
        }

        return _repository.ListarPrediccionesAsync(
            idUsuario,
            cancellationToken);
    }

    private static void ValidarRequest(
        int idEmpresa,
        PronosticoVentaRequest request)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa no es válida.");
        }

        if (request is null)
        {
            throw new ArgumentNullException(
                nameof(request));
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

        if (request.DiasProyeccion < 1 ||
            request.DiasProyeccion > 365)
        {
            throw new ArgumentException(
                "Los días de proyección deben estar entre 1 y 365.");
        }
    }

    private static decimal CalcularNivelConfianza(
        int numeroVentas)
    {
        return numeroVentas switch
        {
            >= 30 => 0.90m,
            >= 15 => 0.80m,
            >= 5 => 0.70m,
            _ => 0.60m
        };
    }
}