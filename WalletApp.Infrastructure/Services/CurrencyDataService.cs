using EcbGateway.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Interfaces;
using WalletApp.Infrastructure.Database;

namespace WalletApp.Infrastructure.Services;

public class CurrencyDataService : ICurrencyDataService
{
    private readonly WalletAppDbContext _dbContext;

    public CurrencyDataService(WalletAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveOrUpdateCurrenciesRates(CurrenciesRates currenciesRates, CancellationToken ct = default)
    {
        var sqlValues = string.Join(",\n", Enumerable.Range(0, currenciesRates.CurrencyRates.Count).Select(i => $"(@Date,@Currency{i},@Rate{i})"));

        var parameters = new List<object>
        {
            new SqlParameter("Date", currenciesRates.Date)
        };

        int i = 0;
        foreach (var cr in currenciesRates.CurrencyRates)
        {
            parameters.Add(new SqlParameter("Currency" + i, cr.Key));
            parameters.Add(new SqlParameter("Rate" + i, cr.Value));
            i++;
        }

#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
        _ = await _dbContext.Database.ExecuteSqlRawAsync($"""
            MERGE CurrencyRate tgt
            USING (VALUES
            {sqlValues}
            ) s(Date,Currency,Rate) ON tgt.Date = s.Date and tgt.Currency = s.Currency
            WHEN MATCHED THEN
                UPDATE SET tgt.Rate = s.Rate
            WHEN NOT MATCHED BY TARGET THEN
                INSERT (Date,Currency,Rate)
                VALUES (s.Date,s.Currency,s.Rate);
            """,
            parameters,
            ct);
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
    }
}
