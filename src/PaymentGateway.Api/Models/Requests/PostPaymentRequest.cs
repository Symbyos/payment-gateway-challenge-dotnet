﻿namespace PaymentGateway.Api.Models.Requests;

public record PostPaymentRequest
{
    public required string CardNumber{ get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required string Cvv { get; init; }
}