using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public record PaymentResponse
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public PaymentStatus Status { get; init; } = PaymentStatus.Rejected;
    public required string CardNumberLastFour { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
}
