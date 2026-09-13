using System.Data;
using Dapper;
using KernixERP.Database.Connections;
using KernixERP.DTOs.Common;
using KernixERP.DTOs.Empleados.Requests;
using KernixERP.DTOs.Empleados.Responses;
using KernixERP.Interfaces.Organizacion;

namespace KernixERP.Repositories.Organizacion;

public sealed class EmpleadoRepository : IEmpleadoRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public EmpleadoRepository(
        ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResponse<EmpleadoResponse>> ListarAsync(
        EmpleadoFiltroRequest filtro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "org.sp_empleado_listar",
            parameters: new
            {
                id_empresa = filtro.IdEmpresa,
                id_sucursal = filtro.IdSucursal,
                estado = filtro.Estado,
                buscar = filtro.Buscar,
                pagina = filtro.Pagina,
                tamano_pagina = filtro.TamanoPagina
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        using var multi =
            await connection.QueryMultipleAsync(command);

        var empleados =
            (await multi.ReadAsync<EmpleadoResponse>())
            .ToList();

        var totalRegistros =
            await multi.ReadSingleOrDefaultAsync<int>();

        return new PagedResponse<EmpleadoResponse>
        {
            Items = empleados,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<EmpleadoDetalleResponse?> ObtenerPorIdAsync(
        int idEmpleado,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "org.sp_empleado_obtener",
            parameters: new
            {
                id_empleado = idEmpleado
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<EmpleadoDetalleResponse>(
                command);
    }

    public async Task<int> CrearAsync(
        EmpleadoCrearRequest request,
        int idUsuarioRegistro,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@id_empresa", request.IdEmpresa);
        parameters.Add("@id_sucursal", request.IdSucursal);
        parameters.Add("@nombres", request.Nombres);
        parameters.Add(
            "@apellido_paterno",
            request.ApellidoPaterno);
        parameters.Add(
            "@apellido_materno",
            request.ApellidoMaterno);
        parameters.Add(
            "@tipo_documento",
            request.TipoDocumento);
        parameters.Add(
            "@numero_documento",
            request.NumeroDocumento);
        parameters.Add("@telefono", request.Telefono);
        parameters.Add("@correo", request.Correo);
        parameters.Add("@cargo", request.Cargo);
        parameters.Add(
            "@fecha_ingreso",
            request.FechaIngreso);
        parameters.Add(
            "@fecha_cese",
            request.FechaCese);
        parameters.Add(
            "@id_usuario_registro",
            idUsuarioRegistro);

        parameters.Add(
            "@id_empleado",
            dbType: DbType.Int32,
            direction: ParameterDirection.Output);

        var command = new CommandDefinition(
            commandText: "org.sp_empleado_crear",
            parameters: parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return parameters.Get<int>("@id_empleado");
    }

    public async Task<bool> ActualizarAsync(
        int idEmpleado,
        EmpleadoActualizarRequest request,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "org.sp_empleado_actualizar",
            parameters: new
            {
                id_empleado = idEmpleado,
                id_empresa = request.IdEmpresa,
                id_sucursal = request.IdSucursal,
                nombres = request.Nombres,
                apellido_paterno = request.ApellidoPaterno,
                apellido_materno = request.ApellidoMaterno,
                tipo_documento = request.TipoDocumento,
                numero_documento = request.NumeroDocumento,
                telefono = request.Telefono,
                correo = request.Correo,
                cargo = request.Cargo,
                fecha_ingreso = request.FechaIngreso,
                fecha_cese = request.FechaCese,
                estado = request.Estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }

    public async Task<bool> CambiarEstadoAsync(
        int idEmpleado,
        bool estado,
        int idUsuarioModifica,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            commandText: "org.sp_empleado_cambiar_estado",
            parameters: new
            {
                id_empleado = idEmpleado,
                estado,
                id_usuario_modifica = idUsuarioModifica
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        return true;
    }
}