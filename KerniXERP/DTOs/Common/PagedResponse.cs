namespace KernixERP.DTOs.Common;

public sealed class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];

    public int Pagina { get; init; }

    public int TamanoPagina { get; init; }

    public int TotalRegistros { get; init; }

    public int TotalPaginas =>
        TamanoPagina <= 0
            ? 0
            : (int)Math.Ceiling(
                TotalRegistros / (double)TamanoPagina);
}