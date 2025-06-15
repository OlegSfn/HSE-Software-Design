using Microsoft.AspNetCore.Mvc;
using PaymentsService.DTOs;
using PaymentsService.Models;
using PaymentsService.Services;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(
        IAccountService accountService,
        ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Account>> GetAccount(string userId)
    {
        try
        {
            var account = await _accountService.GetAccountByUserIdAsync(userId);
            if (account == null)
            {
                return NotFound($"Account for user {userId} not found");
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the account");
        }
    }

    [HttpGet("{userId}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(string userId)
    {
        try
        {
            var balance = await _accountService.GetBalanceAsync(userId);
            return Ok(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the balance");
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var success = await _accountService.CreateAccountAsync(request.UserId, request.InitialBalance);
            if (!success)
            {
                return BadRequest("Account could not be created. It may already exist.");
            }

            return CreatedAtAction(nameof(GetAccount), new { userId = request.UserId }, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account for user {UserId}", request.UserId);
            return StatusCode(500, "An error occurred while creating the account");
        }
    }

    [HttpPost("{userId}/deposit")]
    public async Task<ActionResult> DepositFunds(string userId, [FromBody] DepositRequest request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest("Deposit amount must be greater than zero");
            }

            var (success, error) = await _accountService.DepositFundsAsync(
                userId, request.Amount, request.Description);

            if (!success)
            {
                return BadRequest(error);
            }

            return Ok(new { message = $"Successfully deposited {request.Amount:C} to account" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error depositing funds for user {UserId}", userId);
            return StatusCode(500, "An error occurred while depositing funds");
        }
    }
}