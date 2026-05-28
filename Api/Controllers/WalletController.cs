using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Wallet;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;

[Route("api/wallet")]
[ApiController]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly IWalletTransactionService _walletTransactionService;

    public WalletController(IWalletService walletService, IWalletTransactionService walletTransactionService)
    {
        _walletService = walletService;
        _walletTransactionService = walletTransactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWallet()
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetWalletAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        var userId = User.GetUserId();
        var result = await _walletTransactionService.GetTransactionsAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("banks")]
    public async Task<IActionResult> GetSavedBanks()
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetSavedBanksAsync(userId);
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost("payout")]
    public async Task<IActionResult> RequestPayout([FromBody] PayoutRequestDTO request)
    {
        var userId = User.GetUserId();
        await _walletService.RequestPayoutAsync(userId, request);
        return ApiResponseFactory.Created(new { }, "Payout request submitted");
    }
}