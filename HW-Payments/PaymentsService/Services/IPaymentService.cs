using PaymentsService.Models;
using Shared.Messaging.Messages;

namespace PaymentsService.Services;

public interface IPaymentService
{
    Task<(bool success, string error)> ProcessPaymentRequestAsync(PaymentRequestMessage paymentRequest);
    Task<Transaction> GetPaymentByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetPaymentsByAccountIdAsync(Guid accountId);
    Task<IEnumerable<Transaction>> GetPaymentsByUserIdAsync(string userId);
    Task<IEnumerable<Transaction>> GetAllTransactionsByUserIdAsync(string userId);
}