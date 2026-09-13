namespace KernixERP.Interfaces;

public interface ITokenService
{
    string GenerarRefreshToken();

    string CalcularHash(string token);
}