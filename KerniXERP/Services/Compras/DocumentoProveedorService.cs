using KernixERP.DTOs.Common;
using KernixERP.DTOs.Compras.DocumentosProveedor;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class DocumentoProveedorService
    : IDocumentoProveedorService
{
    private readonly IDocumentoProveedorRepository _repository;

    public DocumentoProveedorService(
        IDocumentoProveedorRepository repository)
    {
        _repository = repository;
    }

    public Task<long> RegistrarAsync(
        int idEmpresa,
        DocumentoProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        if (idEmpresa <= 0)
            throw new ArgumentException(
                "Empresa no válida.");

        request.CodigoTipoDocumento =
            Normalizar(request.CodigoTipoDocumento)
            ?? throw new ArgumentException(
                "Tipo de documento obligatorio.");

        request.CodigoTipoDocumento =
            request.CodigoTipoDocumento
                .PadLeft(2, '0');

        var tiposPermitidos =
            new[]
            {
                "01",
                "03",
                "07",
                "08"
            };

        if (!tiposPermitidos.Contains(
                request.CodigoTipoDocumento))
        {
            throw new ArgumentException(
                "Tipo de documento no permitido.");
        }

        if (request.IdProveedor <= 0)
            throw new ArgumentException(
                "Proveedor no válido.");

        request.NumeroDocumento =
            Normalizar(request.NumeroDocumento)
            ?? throw new ArgumentException(
                "Número de documento obligatorio.");

        request.Moneda =
            Normalizar(request.Moneda)?
                .ToUpperInvariant()
            ?? "PEN";

        request.Motivo =
            Normalizar(request.Motivo);

        request.Observacion =
            Normalizar(request.Observacion);

        request.NumeroDocumentoReferencia =
            Normalizar(
                request.NumeroDocumentoReferencia);

        if (request.Total <= 0)
            throw new ArgumentException(
                "El total debe ser mayor que cero.");

        if (request.FechaEmision == default)
            throw new ArgumentException(
                "Fecha de emisión obligatoria.");

        if (
            request.CodigoTipoDocumento is "07" or "08"
            &&
            (
                request.IdCuentaPorPagar is null
                ||
                request.IdCuentaPorPagar <= 0
            )
        )
        {
            throw new ArgumentException(
                "La Nota de Crédito/Débito requiere una cuenta por pagar.");
        }

        if (
            request.CodigoTipoDocumento is "07" or "08"
            &&
            string.IsNullOrWhiteSpace(request.Motivo)
        )
        {
            throw new ArgumentException(
                "La Nota de Crédito/Débito requiere un motivo.");
        }

        return _repository.RegistrarAsync(
            idEmpresa,
            request,
            cancellationToken);
    }

    public Task<PagedResponse<DocumentoProveedorResponse>>
        ListarAsync(
            int idEmpresa,
            DocumentoProveedorFiltroRequest filtro,
            CancellationToken cancellationToken)
    {
        filtro.Pagina =
            Math.Max(
                filtro.Pagina,
                1);

        filtro.TamanoPagina =
            Math.Clamp(
                filtro.TamanoPagina,
                1,
                100);

        filtro.CodigoTipo =
            Normalizar(filtro.CodigoTipo);

        if (filtro.CodigoTipo is not null)
        {
            filtro.CodigoTipo =
                filtro.CodigoTipo
                    .PadLeft(2, '0');
        }

        filtro.Estado =
            Normalizar(filtro.Estado)?
                .ToLowerInvariant();

        filtro.Buscar =
            Normalizar(filtro.Buscar);

        return _repository.ListarAsync(
            idEmpresa,
            filtro,
            cancellationToken);
    }

    public Task<DocumentoProveedorDetalleResponse?> ObtenerAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idDocumentoProveedor <= 0)
            return Task.FromResult<
                DocumentoProveedorDetalleResponse?>(
                null);

        return _repository.ObtenerAsync(
            idDocumentoProveedor,
            idEmpresa,
            cancellationToken);
    }

    public async Task AplicarAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        var documento =
            await _repository.ObtenerAsync(
                idDocumentoProveedor,
                idEmpresa,
                cancellationToken);

        if (documento is null)
            throw new KeyNotFoundException(
                "Documento no encontrado.");

        switch (documento.CodigoTipoDocumento)
        {
            case "07":

                await _repository
                    .AplicarNotaCreditoAsync(
                        idDocumentoProveedor,
                        idEmpresa,
                        cancellationToken);

                break;

            case "08":

                await _repository
                    .AplicarNotaDebitoAsync(
                        idDocumentoProveedor,
                        idEmpresa,
                        cancellationToken);

                break;

            case "01":
            case "03":

                throw new InvalidOperationException(
                    "Factura y Boleta no requieren la operación aplicar.");

            default:

                throw new InvalidOperationException(
                    "Tipo de documento no soportado.");
        }
    }

    public async Task AnularAsync(
        long idDocumentoProveedor,
        int idEmpresa,
        DocumentoProveedorAnularRequest request,
        CancellationToken cancellationToken)
    {
        var observacion =
            Normalizar(request.Observacion)
            ?? throw new ArgumentException(
                "Motivo de anulación obligatorio.");

        var documento =
            await _repository.ObtenerAsync(
                idDocumentoProveedor,
                idEmpresa,
                cancellationToken);

        if (documento is null)
            throw new KeyNotFoundException(
                "Documento no encontrado.");

        switch (documento.CodigoTipoDocumento)
        {
            case "07":

                await _repository
                    .AnularNotaCreditoAsync(
                        idDocumentoProveedor,
                        idEmpresa,
                        observacion,
                        cancellationToken);

                break;

            case "08":

                await _repository
                    .AnularNotaDebitoAsync(
                        idDocumentoProveedor,
                        idEmpresa,
                        observacion,
                        cancellationToken);

                break;

            case "01":
            case "03":

                throw new InvalidOperationException(
                    "La anulación de Factura/Boleta se implementará mediante su flujo documental correspondiente.");

            default:

                throw new InvalidOperationException(
                    "Tipo de documento no soportado.");
        }
    }

    private static string? Normalizar(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}