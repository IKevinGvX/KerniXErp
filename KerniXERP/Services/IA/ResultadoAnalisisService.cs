// Services/IA/ResultadoAnalisisService.cs

using KernixERP.DTOs.IA;
using KernixERP.Interfaces.IA;

namespace KernixERP.Services.IA;

public sealed class ResultadoAnalisisService
    : IResultadoAnalisisService
{
    private readonly IResultadoAnalisisRepository _repository;

    public ResultadoAnalisisService(
        IResultadoAnalisisRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResultadoAnalisisResponse> RegistrarAsync(
        int idUsuario,
        ResultadoAnalisisRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (idUsuario <= 0)
            throw new ArgumentException(
                "El usuario no es válido.");

        if (request.IdModeloIA <= 0)
            throw new ArgumentException(
                "El modelo de IA no es válido.");

        if (string.IsNullOrWhiteSpace(request.Modulo))
            throw new ArgumentException(
                "El módulo es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.TipoAnalisis))
            throw new ArgumentException(
                "El tipo de análisis es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Resultado))
            throw new ArgumentException(
                "El resultado es obligatorio.");

        if (request.PuntajeRiesgo.HasValue &&
            request.PuntajeRiesgo is < 0 or > 100)
        {
            throw new ArgumentException(
                "El puntaje debe estar entre 0 y 100.");
        }

        return await _repository.RegistrarAsync(
                   idUsuario,
                   request,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "No fue posible registrar el resultado del análisis.");
    }

    public Task<IReadOnlyCollection<ResultadoAnalisisResponse>>
        ListarAsync(
            int idUsuario,
            ResultadoAnalisisFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        if (idUsuario <= 0)
            throw new ArgumentException(
                "El usuario no es válido.");

        return _repository.ListarAsync(
            idUsuario,
            filtro,
            cancellationToken);
    }
}