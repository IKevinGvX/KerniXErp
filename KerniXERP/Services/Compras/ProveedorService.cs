using KernixERP.DTOs.Proveedores.Requests;
using KernixERP.DTOs.Proveedores.Responses;
using KernixERP.Interfaces.Compras;

namespace KernixERP.Services.Compras;

public sealed class ProveedorService : IProveedorService
{
    private static readonly HashSet<string> TiposDocumentoPermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "ruc",
            "dni",
            "ce",
            "pasaporte"
        };

    private readonly IProveedorRepository _repository;

    public ProveedorService(
        IProveedorRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<ProveedorResponse>> ListarAsync(
        ProveedorFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        if (filtro.IdEmpresa is null ||
            filtro.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        filtro.Busqueda =
            NormalizarTexto(filtro.Busqueda);

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<ProveedorDetalleResponse?> ObtenerPorIdAsync(
        int idProveedor,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idProveedor <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult<
                ProveedorDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idProveedor,
            idEmpresa,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        ProveedorCrearRequest request,
        CancellationToken cancellationToken)
    {
        Normalizar(request);
        ValidarProveedor(
            request.IdEmpresa,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.RazonSocial,
            request.Contactos,
            request.Direcciones);

        return _repository.CrearAsync(
            request,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idProveedor,
        int idEmpresa,
        ProveedorActualizarRequest request,
        CancellationToken cancellationToken)
    {
        if (idProveedor <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult(false);
        }

        Normalizar(request);
        ValidarProveedor(
            idEmpresa,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.RazonSocial,
            request.Contactos,
            request.Direcciones);

        return _repository.ActualizarAsync(
            idProveedor,
            idEmpresa,
            request,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idProveedor,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken)
    {
        if (idProveedor <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idProveedor,
            idEmpresa,
            estado,
            cancellationToken);
    }

    private static void ValidarProveedor(
        int idEmpresa,
        string tipoDocumento,
        string numeroDocumento,
        string razonSocial,
        IEnumerable<ContactoProveedorRequest>? contactos,
        IEnumerable<DireccionProveedorRequest>? direcciones)
    {
        if (idEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (!TiposDocumentoPermitidos.Contains(
                tipoDocumento))
        {
            throw new ArgumentException(
                "El tipo de documento no es válido.");
        }

        if (string.IsNullOrWhiteSpace(
                razonSocial))
        {
            throw new ArgumentException(
                "La razón social es obligatoria.");
        }

        ValidarDocumento(
            tipoDocumento,
            numeroDocumento);

        var contactosPrincipales =
            contactos?
                .Count(contacto =>
                    contacto.EsPrincipal)
            ?? 0;

        if (contactosPrincipales > 1)
        {
            throw new ArgumentException(
                "Solo puede existir un contacto principal.");
        }

        var direccionesPrincipales =
            direcciones?
                .Count(direccion =>
                    direccion.EsPrincipal)
            ?? 0;

        if (direccionesPrincipales > 1)
        {
            throw new ArgumentException(
                "Solo puede existir una dirección principal.");
        }

        foreach (var contacto in contactos ?? [])
        {
            if (string.IsNullOrWhiteSpace(
                    contacto.Nombres))
            {
                throw new ArgumentException(
                    "El nombre del contacto es obligatorio.");
            }
        }

        foreach (var direccion in direcciones ?? [])
        {
            if (string.IsNullOrWhiteSpace(
                    direccion.Direccion))
            {
                throw new ArgumentException(
                    "La dirección del proveedor es obligatoria.");
            }

            if (!string.IsNullOrWhiteSpace(
                    direccion.Ubigeo) &&
                direccion.Ubigeo.Length != 6)
            {
                throw new ArgumentException(
                    "El ubigeo debe contener exactamente 6 caracteres.");
            }
        }
    }

    private static void ValidarDocumento(
        string tipoDocumento,
        string numeroDocumento)
    {
        if (string.IsNullOrWhiteSpace(
                numeroDocumento))
        {
            throw new ArgumentException(
                "El número de documento es obligatorio.");
        }

        var documento = numeroDocumento.Trim();

        switch (tipoDocumento.ToLowerInvariant())
        {
            case "ruc":
                if (documento.Length != 11 ||
                    !documento.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "El RUC debe contener 11 dígitos.");
                }

                break;

            case "dni":
                if (documento.Length != 8 ||
                    !documento.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "El DNI debe contener 8 dígitos.");
                }

                break;

            case "ce":
            case "pasaporte":
                if (documento.Length < 6)
                {
                    throw new ArgumentException(
                        "El documento debe contener al menos 6 caracteres.");
                }

                break;
        }
    }

    private static void Normalizar(
        ProveedorCrearRequest request)
    {
        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToLowerInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.RazonSocial =
            request.RazonSocial.Trim();

        request.NombreComercial =
            NormalizarTexto(
                request.NombreComercial);

        request.Telefono =
            NormalizarTexto(
                request.Telefono);

        request.Correo =
            NormalizarTexto(
                request.Correo)?
                .ToLowerInvariant();

        NormalizarContactos(
            request.Contactos);

        NormalizarDirecciones(
            request.Direcciones);
    }

    private static void Normalizar(
        ProveedorActualizarRequest request)
    {
        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToLowerInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.RazonSocial =
            request.RazonSocial.Trim();

        request.NombreComercial =
            NormalizarTexto(
                request.NombreComercial);

        request.Telefono =
            NormalizarTexto(
                request.Telefono);

        request.Correo =
            NormalizarTexto(
                request.Correo)?
                .ToLowerInvariant();

        NormalizarContactos(
            request.Contactos);

        NormalizarDirecciones(
            request.Direcciones);
    }

    private static void NormalizarContactos(
        IEnumerable<ContactoProveedorRequest>? contactos)
    {
        foreach (var contacto in contactos ?? [])
        {
            contacto.Nombres =
                contacto.Nombres.Trim();

            contacto.Cargo =
                NormalizarTexto(
                    contacto.Cargo);

            contacto.Telefono =
                NormalizarTexto(
                    contacto.Telefono);

            contacto.Correo =
                NormalizarTexto(
                    contacto.Correo)?
                    .ToLowerInvariant();
        }
    }

    private static void NormalizarDirecciones(
        IEnumerable<DireccionProveedorRequest>? direcciones)
    {
        foreach (var direccion in direcciones ?? [])
        {
            direccion.TipoDireccion =
                NormalizarTexto(
                    direccion.TipoDireccion);

            direccion.Direccion =
                direccion.Direccion.Trim();

            direccion.Referencia =
                NormalizarTexto(
                    direccion.Referencia);

            direccion.Distrito =
                NormalizarTexto(
                    direccion.Distrito);

            direccion.Provincia =
                NormalizarTexto(
                    direccion.Provincia);

            direccion.Departamento =
                NormalizarTexto(
                    direccion.Departamento);

            direccion.Ubigeo =
                NormalizarTexto(
                    direccion.Ubigeo);
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