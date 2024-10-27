using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WalletApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly ILogger<WalletsController> _logger;

    public WalletsController(ILogger<WalletsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CreateWallet()
    {
        return await Task.FromResult(Ok());
    }

    [HttpGet("{walletId}")]
    public async Task<ActionResult> RetrieveWalletBalance(
        long walletId,
        string? currency)
    {
        return await Task.FromResult(Ok());
    }

    [HttpPost("{walletId}/adjustBalance")]
    public async Task<ActionResult> AdjustWalletBalance(
        long walletId,
        decimal amount,
        string? currency,
        string strategy)
    {
        return await Task.FromResult(Ok());
    }
}
