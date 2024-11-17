using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Helpers;

/// <summary>
/// <c>PaymentHelper</c> encapsulates functions to help with validating and requesting payment requests to the acquiring bank.
/// </summary>
public static class PaymentHelper
{
    private const int CARD_NUM_MIN_LENGTH = 14;
    private const int CARD_NUM_MAX_LENGTH = 19;

    /// <summary>
    /// List of supported currencies iso codes.
    /// </summary>
    private readonly static List<string> _isoCurrencyCodes = ["GBP", "USD", "EUR"];

    /// <summary>
    /// <c>GetLastFourCardDigits</c> retrieves the last 4 digits of a <c>PostPaymentRequest</c> card number.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static string GetLastFourCardDigits(this PostPaymentRequest ppr)
    {
        if (ppr.ValidateCardNumber())
        {
            return ppr.CardNumber[(ppr.CardNumber.Length - 4)..];
        }

        return string.Empty;
    }

    /// <summary>
    /// <c>ValidateCardNumber</c> verifies that the card number is composed of 14 to 19 digits.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static bool ValidateCardNumber(this PostPaymentRequest ppr)
    {
        // Between 14-19 characters long
        // Must only contain numeric characters
        if (ppr.CardNumber.All(Char.IsDigit) && ppr.CardNumber.Length <= CARD_NUM_MAX_LENGTH && ppr.CardNumber.Length >= CARD_NUM_MIN_LENGTH)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// <c>ValidateExpiryDate</c> verifies that the expiry date is set in the future.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static bool ValidateExpiryDate(this PostPaymentRequest ppr)
    {
        if (ppr.ExpiryMonth >= 1 && ppr.ExpiryMonth <= 12 && ppr.ExpiryYear >= DateTime.UtcNow.Year)
        {
            // Value must be in the future
            // A card is valid until the end of the last day of the month
            DateTime expiryDate = new DateTime(ppr.ExpiryYear, ppr.ExpiryMonth, 1).AddMonths(1);
            if (DateTime.UtcNow < expiryDate)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// <c>ValidateCvv</c> verifies that the Cvv is composed of 3 to 4 digits.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static bool ValidateCvv(this PostPaymentRequest ppr)
    {
        if (ppr.Cvv.All(Char.IsDigit) && ppr.Cvv.Length <= 4 && ppr.Cvv.Length >= 3)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// <c>ValidatePaymentRequest</c> validates the fields of a <c>PostPaymentRequest</c>.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static bool ValidatePaymentRequest(this PostPaymentRequest ppr)
    {

        if (ppr.ValidateCardNumber() && ppr.ValidateExpiryDate() && _isoCurrencyCodes.Contains(ppr.Currency) && ppr.ValidateCvv())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// <c>ToAcquiringBankRequest</c> constructs a request object to be sent to the acquiring bank API.
    /// </summary>
    /// <param name="ppr"></param>
    /// <returns></returns>
    public static object ToAcquiringBankRequest(this PostPaymentRequest ppr)
    {
        var postAcquiringBankRequest = new
        {
            card_number = ppr.CardNumber,
            expiry_date = string.Format("{0:00}", ppr.ExpiryMonth) + "/" + ppr.ExpiryYear,
            currency = ppr.Currency,
            amount = ppr.Amount,
            cvv = ppr.Cvv
        };

        return postAcquiringBankRequest;
    }
}