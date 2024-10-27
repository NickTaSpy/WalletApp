using System;
using System.Collections.Generic;

namespace EcbGateway.Models;

public class CurrenciesRates
{
    public DateTime Date { get; set; }
    public IDictionary<string, decimal> CurrencyRates { get; set; } = new Dictionary<string, decimal>();
}
