namespace PaymentGateway.Api.Enums;

/// <summary>
/// Enumeration <c>PaymentStatus</c> describes the status of a <c>PaymentResponse</c>.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// The status <c>Authorized</c> indicates that the payment was authorized by the call to the acquiring bank.
    /// </summary>
    Authorized,
    /// <summary>
    /// The status <c>Declined</c> indicates that the payment was declined by the call to the acquiring bank.
    /// </summary>
    Declined,
    /// <summary>
    /// The status <c>Rejected</c> indicates that no payment could be created as invalid information was supplied to the payment gateway and therefore it has rejected the request without calling the acquiring bank.
    /// </summary>
    Rejected
}