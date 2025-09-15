using Microsoft.AspNetCore.Mvc;
using PaymentsService.Models;
using PaymentsService.Services;
using Shared.Messaging.Messages;

namespace PaymentsService.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetPayment(Guid id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound($"Payment with ID {id} not found");
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with ID {PaymentId}", id);
            return StatusCode(500, "An error occurred while retrieving the payment");
        }
    }

    [HttpGet("user/{userId}/payments")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetPaymentsByUserId(string userId)
    {
        try
        {
            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the payments");
        }
    }

    [HttpGet("user/{userId}/all-transactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactionsByUserId(string userId)
    {
        try
        {
            _logger.LogInformation("Getting all transactions for user {UserId}", userId);
            var transactions = await _paymentService.GetAllTransactionsByUserIdAsync(userId);

            if (!transactions.Any())
            {
                _logger.LogWarning("No transactions found for user {UserId}", userId);
                return Ok(new List<Transaction>());
            }

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all transactions for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the transactions");
        }
    }
}