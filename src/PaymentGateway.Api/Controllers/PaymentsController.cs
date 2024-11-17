using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly ILogger<PaymentsController> _logger;
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentsController(ILogger<PaymentsController> logger, PaymentsRepository paymentsRepository, IHttpClientFactory httpClientFactory)
    {
        _paymentsRepository = paymentsRepository;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// <c>GetPaymentAsync</c> processes get request and returns a <c>PaymentResponse</c>. If not found, returns <c>null</c> and <c>NotFoundObjectResult</c>.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public ActionResult<PaymentResponse?> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.GetPayment(id);

        if (payment == null)
        {
            return new NotFoundObjectResult(payment);
        }
        else
        {
            return new OkObjectResult(payment);
        }
    }

    /// <summary>
    /// <c>PostPaymentAsync</c> processes post request to forward a <c>PostPaymentRequest</c> to the acquiring bank.
    /// </summary>
    /// <param name="postPaymentRequest"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException">During transaction with the acquiring bank server, if an error happens this exception will be raised. The request will be stated as Rejected.</exception>
    /// <exception cref="Exception">For any exception case not handled previously, an <c>Exception</c> will be raised. The request will be stated as Rejected.</exception>
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> PostPaymentAsync(PostPaymentRequest postPaymentRequest)
    {
        PaymentStatus status = PaymentStatus.Rejected;

        try
        {
            if (postPaymentRequest.ValidatePaymentRequest())
            {
                HttpResponseMessage response = await _httpClientFactory.CreateClient().PostAsJsonAsync("http://localhost:8080/payments", postPaymentRequest.ToAcquiringBankRequest());
                if (response.IsSuccessStatusCode)
                {
                    AcquiringBankResponse bankResponse = await response.Content.ReadFromJsonAsync<AcquiringBankResponse>();
                    if (bankResponse == null)
                    {
                        status = PaymentStatus.Declined;
                    }
                    else
                    {
                        status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
                    }
                }
                else
                {
                    status = PaymentStatus.Declined;
                }
            }
        }
        catch(HttpRequestException ex)
        {
            _logger.LogError($"{DateTime.UtcNow}:Error with bank endpoint. Detail:{ex.Message}");
            status = PaymentStatus.Rejected;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{DateTime.UtcNow}:Error during payment request processing. Detail:{ex.Message}");
            status = PaymentStatus.Rejected;
        }

        PaymentResponse postPaymentResponse = new()
        {
            CardNumberLastFour = postPaymentRequest.GetLastFourCardDigits(),
            ExpiryMonth = postPaymentRequest.ExpiryMonth,
            ExpiryYear = postPaymentRequest.ExpiryYear,
            Currency = postPaymentRequest.Currency,
            Amount = postPaymentRequest.Amount,
            Status = status
        };

        _paymentsRepository.AddPayment(postPaymentResponse);

        return new OkObjectResult(postPaymentResponse);
    }
}