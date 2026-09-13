using KernixERP.DTOs.Clientes.Requests;
using KernixERP.DTOs.Clientes.Responses;
using KernixERP.DTOs.Common;
using KernixERP.Interfaces.Ventas;

namespace KernixERP.Services.Ventas;

public sealed class ClienteService : IClienteService
{
    private static readonly HashSet<string> TiposClientePermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "natural",
            "juridico"
        };

    private static readonly HashSet<string> TiposDocumentoPermitidos =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "dni",
            "ruc",
            "ce",
            "pasaporte"
        };

    private readonly IClienteRepository _repository;

    public ClienteService(
        IClienteRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResponse<ClienteResponse>> ListarAsync(
        ClienteFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        filtro.Pagina = Math.Max(
            filtro.Pagina,
            1);

        filtro.TamanoPagina = Math.Clamp(
            filtro.TamanoPagina,
            1,
            100);

        filtro.TipoCliente =
            NormalizarTexto(filtro.TipoCliente)?
                .ToLowerInvariant();

        filtro.TipoDocumento =
            NormalizarTexto(filtro.TipoDocumento)?
                .ToLowerInvariant();

        filtro.Buscar =
            NormalizarTexto(filtro.Buscar);

        if (filtro.IdEmpresa is null ||
            filtro.IdEmpresa <= 0)
        {
            throw new ArgumentException(
                "La empresa es obligatoria.");
        }

        if (filtro.TipoCliente is not null &&
            !TiposClientePermitidos.Contains(
                filtro.TipoCliente))
        {
            throw new ArgumentException(
                "El tipo de cliente debe ser natural o juridico.");
        }

        if (filtro.TipoDocumento is not null &&
            !TiposDocumentoPermitidos.Contains(
                filtro.TipoDocumento))
        {
            throw new ArgumentException(
                "El tipo de documento no es válido.");
        }

        return _repository.ListarAsync(
            filtro,
            cancellationToken);
    }

    public Task<ClienteDetalleResponse?> ObtenerPorIdAsync(
        int idCliente,
        int idEmpresa,
        CancellationToken cancellationToken)
    {
        if (idCliente <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult<
                ClienteDetalleResponse?>(null);
        }

        return _repository.ObtenerPorIdAsync(
            idCliente,
            idEmpresa,
            cancellationToken);
    }

    public Task<int> CrearAsync(
        ClienteCrearRequest request,
        CancellationToken cancellationToken)
    {
        Normalizar(request);
        ValidarDatosCliente(
            request.TipoCliente,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.RazonSocial,
            request.Nombres,
            request.Apellidos,
            request.Contactos,
            request.Direcciones);

        return _repository.CrearAsync(
            request,
            cancellationToken);
    }

    public Task<bool> ActualizarAsync(
        int idCliente,
        ClienteActualizarRequest request,
        CancellationToken cancellationToken)
    {
        if (idCliente <= 0)
        {
            return Task.FromResult(false);
        }

        Normalizar(request);
        ValidarDatosCliente(
            request.TipoCliente,
            request.TipoDocumento,
            request.NumeroDocumento,
            request.RazonSocial,
            request.Nombres,
            request.Apellidos,
            request.Contactos,
            request.Direcciones);

        return _repository.ActualizarAsync(
            idCliente,
            request,
            cancellationToken);
    }

    public Task<bool> CambiarEstadoAsync(
        int idCliente,
        int idEmpresa,
        bool estado,
        CancellationToken cancellationToken)
    {
        if (idCliente <= 0 ||
            idEmpresa <= 0)
        {
            return Task.FromResult(false);
        }

        return _repository.CambiarEstadoAsync(
            idCliente,
            idEmpresa,
            estado,
            cancellationToken);
    }

    private static void ValidarDatosCliente(
        string tipoCliente,
        string tipoDocumento,
        string numeroDocumento,
        string? razonSocial,
        string? nombres,
        string? apellidos,
        IEnumerable<ContactoClienteRequest>? contactos,
        IEnumerable<DireccionClienteRequest>? direcciones)
    {
        if (!TiposClientePermitidos.Contains(
                tipoCliente))
        {
            throw new ArgumentException(
                "El tipo de cliente debe ser natural o juridico.");
        }

        if (!TiposDocumentoPermitidos.Contains(
                tipoDocumento))
        {
            throw new ArgumentException(
                "El tipo de documento no es válido.");
        }

        ValidarDocumento(
            tipoDocumento,
            numeroDocumento);

        if (tipoCliente.Equals(
                "juridico",
                StringComparison.OrdinalIgnoreCase) &&
            string.IsNullOrWhiteSpace(razonSocial))
        {
            throw new ArgumentException(
                "La razón social es obligatoria para clientes jurídicos.");
        }

        if (tipoCliente.Equals(
                "natural",
                StringComparison.OrdinalIgnoreCase) &&
            (
                string.IsNullOrWhiteSpace(nombres) ||
                string.IsNullOrWhiteSpace(apellidos)
            ))
        {
            throw new ArgumentException(
                "Los nombres y apellidos son obligatorios para clientes naturales.");
        }

        var contactosPrincipales =
            contactos?
                .Count(contacto =>
                    contacto.EsPrincipal &&
                    contacto.Estado)
            ?? 0;

        if (contactosPrincipales > 1)
        {
            throw new ArgumentException(
                "Solo puede existir un contacto principal.");
        }

        var direccionesPrincipales =
            direcciones?
                .Count(direccion =>
                    direccion.EsPrincipal &&
                    direccion.Estado)
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
                    "La dirección del cliente es obligatoria.");
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

        var documento =
            numeroDocumento.Trim();

        switch (tipoDocumento.ToLowerInvariant())
        {
            case "dni":
                if (documento.Length != 8 ||
                    !documento.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "El DNI debe contener 8 dígitos.");
                }

                break;

            case "ruc":
                if (documento.Length != 11 ||
                    !documento.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "El RUC debe contener 11 dígitos.");
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
        ClienteCrearRequest request)
    {
        request.TipoCliente =
            request.TipoCliente
                .Trim()
                .ToLowerInvariant();

        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToLowerInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.RazonSocial =
            NormalizarTexto(request.RazonSocial);

        request.Nombres =
            NormalizarTexto(request.Nombres);

        request.Apellidos =
            NormalizarTexto(request.Apellidos);

        request.Telefono =
            NormalizarTexto(request.Telefono);

        request.Correo =
            NormalizarTexto(request.Correo)?
                .ToLowerInvariant();

        NormalizarContactos(
            request.Contactos);

        NormalizarDirecciones(
            request.Direcciones);
    }

    private static void Normalizar(
        ClienteActualizarRequest request)
    {
        request.TipoCliente =
            request.TipoCliente
                .Trim()
                .ToLowerInvariant();

        request.TipoDocumento =
            request.TipoDocumento
                .Trim()
                .ToLowerInvariant();

        request.NumeroDocumento =
            request.NumeroDocumento.Trim();

        request.RazonSocial =
            NormalizarTexto(request.RazonSocial);

        request.Nombres =
            NormalizarTexto(request.Nombres);

        request.Apellidos =
            NormalizarTexto(request.Apellidos);

        request.Telefono =
            NormalizarTexto(request.Telefono);

        request.Correo =
            NormalizarTexto(request.Correo)?
                .ToLowerInvariant();

        NormalizarContactos(
            request.Contactos);

        NormalizarDirecciones(
            request.Direcciones);
    }

    private static void NormalizarContactos(
        IEnumerable<ContactoClienteRequest>? contactos)
    {
        foreach (var contacto in contactos ?? [])
        {
            contacto.Nombres =
                contacto.Nombres.Trim();

            contacto.Cargo =
                NormalizarTexto(contacto.Cargo);

            contacto.Telefono =
                NormalizarTexto(contacto.Telefono);

            contacto.Correo =
                NormalizarTexto(contacto.Correo)?
                    .ToLowerInvariant();
        }
    }

    private static void NormalizarDirecciones(
        IEnumerable<DireccionClienteRequest>? direcciones)
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