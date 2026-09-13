using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Sucursales.Requests;
using KernixERP.DTOs.Sucursales.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Repositories.Organizacion;

public sealed class SucursalRepository : ISucursalRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SucursalRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<SucursalResponse>> ListarAsync(
        SucursalFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            "org.sp_sucursal_listar",
            new
            {
                id_empresa = filtro.IdEmpresa,
                estado = filtro.Estado,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);

        var items = (await multi.ReadAsync<SucursalResponse>()).ToList();
        var total = await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<SucursalResponse>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = total
        };
    }

    public async Task<SucursalDetalleResponse?> ObtenerPorIdAsync(
        int idSucursal,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<SucursalDetalleResponse>(
            new CommandDefinition(
                "org.sp_sucursal_obtener",
                new { id_sucursal = idSucursal },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<int> CrearAsync(
        SucursalCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@id_empresa", request.IdEmpresa);
        parameters.Add("@codigo", request.Codigo);
        parameters.Add("@nombre", request.Nombre);
        parameters.Add("@direccion", request.Direccion);
        parameters.Add("@telefono", request.Telefono);
        parameters.Add("@correo", request.Correo);
        parameters.Add("@ubigeo", request.Ubigeo);
        parameters.Add("@id_usuario_registro", idUsuarioRegistro);

        parameters.Add(
            "@id_sucursal",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "org.sp_sucursal_crear",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return parameters.Get<int>("@id_sucursal");
    }

    public async Task<bool> ActualizarAsync(
        int idSucursal,
        SucursalActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "org.sp_sucursal_actualizar",
                new
                {
                    id_sucursal = idSucursal,
                    id_empresa = request.IdEmpresa,
                    codigo = request.Codigo,
                    nombre = request.Nombre,
                    direccion = request.Direccion,
                    telefono = request.Telefono,
                    correo = request.Correo,
                    ubigeo = request.Ubigeo,
                    estado = request.Estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idSucursal,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                "org.sp_sucursal_cambiar_estado",
                new
                {
                    id_sucursal = idSucursal,
                    estado,
                    id_usuario_modifica = idUsuarioModifica
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return true;
    }
}