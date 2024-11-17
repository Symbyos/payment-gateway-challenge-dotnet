using System.Collections.Concurrent;

using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

/// <summary>
/// <c>PaymentsRepository</c> to store and retrieve <c>PaymentResponse</c>.
/// </summary>
public class PaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, PaymentResponse> _payments = new();
    
    /// <summary>
    /// Add a <c>PaymentResponse</c> to the repository.
    /// </summary>
    /// <param name="payment"></param>
    public void AddPayment(PaymentResponse payment)
    {
        _payments[payment.Id] = payment;
    }

    /// <summary>
    /// Get a <c>PaymentResponse</c> from the repository referenced by its <c>Guid</c>.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PaymentResponse? GetPayment(Guid id)
    {
        _payments.TryGetValue(id, out var response);
        return response;
    }
}