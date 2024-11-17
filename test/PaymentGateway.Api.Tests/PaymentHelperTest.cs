using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests;
public class PaymentHelperTest
{
    [Theory]
    [InlineData("2222405343248877", true)]
    [InlineData("2222405343248112", true)]
    [InlineData("22224053432488772222405343248877", false)]
    [InlineData("222240534324887a", false)]
    [InlineData("1234", false)]
    public void ValidateCardNumber(string cardNumber, bool expected)
    {
        var ppr = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 10,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 100,
            Cvv = "1234"
        };

        Assert.Equal(ppr.ValidateCardNumber(), expected);
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("1234", true)]
    [InlineData("123a", false)]
    [InlineData("1", false)]
    [InlineData("12345", false)]
    public void ValidateCvv(string cvv, bool expected)
    {
        var ppr = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 10,
            ExpiryYear = 2025,
            Currency = "GBP",
            Amount = 100,
            Cvv = cvv
        };

        Assert.Equal(ppr.ValidateCvv(), expected);
    }

    [Theory]
    [InlineData(5, 2025, true)]
    [InlineData(12, 2024, true)]
    [InlineData(10, 2024, false)]
    [InlineData(-1, 2025, false)]
    [InlineData(12, -1, false)]
    public void ValidateExpiryDate(int expiryMonth, int expiryYear, bool expected)
    {
        var ppr = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = "GBP",
            Amount = 100,
            Cvv = "1234"
        };

        Assert.Equal(ppr.ValidateExpiryDate(), expected);
    }

    [Theory]
    [InlineData("2222405343248877", 4, 2025, "GBP", 100, "123", true)]
    [InlineData("2222405343248112", 1, 2026, "USD", 60000, "456", true)]
    [InlineData("22224053432488772222405343248877", 4, 2025, "GBP", 100, "123", false)]
    [InlineData("222240534324887a", 4, 2025, "GBP", 100, "123", false)]
    [InlineData("2222405343248877", 13, 2025, "GBP", 100, "123", false)]
    [InlineData("2222405343248877", 4, 2024, "GBP", 100, "123", false)]
    [InlineData("2222405343248877", 10, 2025, "JPY", 100, "123", false)]
    [InlineData("2222405343248877", 10, 2025, "GBP", 100, "12345", false)]
    [InlineData("2222405343248877", 10, 2025, "GBP", 100, "123a", false)]
    public void ValidatePaymentRequest(string cardNumber, int expiryMonth, int expiryYear, string currency, int amount, string cvv, bool expected)
    {
        var ppr = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = cvv
        };

        Assert.Equal(ppr.ValidatePaymentRequest(), expected);
    }
}
