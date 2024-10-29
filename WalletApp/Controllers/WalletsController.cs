using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using WalletApp.Core.Interfaces;
using WalletApp.Core.Models;
using WalletApp.Core.Models.Requests;

namespace WalletApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly ILogger<WalletsController> _logger;
    private readonly IWalletService _walletService;

    public WalletsController(
        ILogger<WalletsController> logger,
        IWalletService walletService)
    {
        _logger = logger;
        _walletService = walletService;
    }

    [HttpPost]
    public async Task<ActionResult<Wallet>> CreateWallet(
        [FromBody] CreateWalletRequest request,
        CancellationToken ct = default)
    {
        var wallet =  await _walletService.CreateWallet(request, ct);
        return Ok(wallet);
    }

    [HttpGet("{walletId}")]
    public async Task<ActionResult<decimal>> RetrieveWalletBalance(
        [FromRoute] long walletId,
        [FromQuery] string? currency,
        CancellationToken ct = default)
    {
        var balance = await _walletService.RetrieveWalletBalance(walletId, currency, ct);
        return Ok(balance);
    }

    [HttpPost("{walletId}/adjustBalance")]
    public async Task<ActionResult> AdjustWalletBalance(
        [FromRoute] long walletId,
        [FromQuery] AdjustWalletBalanceRequest request,
        CancellationToken ct = default)
    {
        var balance = await _walletService.AdjustWalletBalance(walletId, request, ct);
        return Ok(balance);
    }
}
