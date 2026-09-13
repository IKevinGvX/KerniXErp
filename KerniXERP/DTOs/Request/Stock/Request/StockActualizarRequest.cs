using System.ComponentModel.DataAnnotations;

namespace KernixERP.DTOs.Stock.Requests;

public sealed class StockActualizarRequest
{
    [Range(0, 999999999999.9999)]
    public decimal StockMinimo { get; set; }

    [Range(0, 999999999999.9999)]
    public decimal? StockMaximo { get; set; }
}