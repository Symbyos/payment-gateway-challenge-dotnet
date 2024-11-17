using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Helpers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

/// <summary>
/// Test the <c>PaymentsController</c> workflow. <c>PostPaymentRequest</c> tests will fail if the bank simulator is offline.
/// </summary>
public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPayment_Successfully()
    {
        PaymentResponse payment = new()
        {
            Status = Enums.PaymentStatus.Authorized,
            ExpiryYear = _random.Next(2025, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.AddPayment(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(paymentResponse.Id, payment.Id);
    }

    [Fact]
    public async Task IfPaymentNotFound_Returns404()
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPaymentRequest_RejectedSuccessfully()
    {
        PostPaymentRequest ppr = new()
        {
            CardNumber = "4444444444444444a",
            ExpiryYear = _random.Next(DateTime.UtcNow.Year, 2030),
            ExpiryMonth = _random.Next(DateTime.UtcNow.Month, 12),
            Amount = _random.Next(1, 10000),
            Currency = "GBP",
            Cvv = "333"
        };

        var paymentsRepository = new PaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        var response = await client.PostAsJsonAsync("/api/Payments", ppr);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(paymentResponse.Status, Enums.PaymentStatus.Rejected);
        Assert.Equal(paymentResponse.CardNumberLastFour, ppr.GetLastFourCardDigits());
        Assert.Equal(paymentResponse.ExpiryYear, ppr.ExpiryYear);
        Assert.Equal(paymentResponse.Amount, ppr.Amount);
        Assert.Equal(paymentResponse.Currency, ppr.Currency);
    }

    [Fact]
    public async Task PostPaymentRequest_DeclinedSuccessfully()
    {
        PostPaymentRequest ppr = new()
        {
            CardNumber = "4444444444444444",
            ExpiryYear = _random.Next(DateTime.UtcNow.Year, 2030),
            ExpiryMonth = _random.Next(DateTime.UtcNow.Month, 12),
            Amount = _random.Next(1, 10000),
            Currency = "GBP",
            Cvv = "333"
        };

        var paymentsRepository = new PaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        var response = await client.PostAsJsonAsync("/api/Payments", ppr);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(paymentResponse.Status, Enums.PaymentStatus.Declined);
        Assert.Equal(paymentResponse.CardNumberLastFour, ppr.GetLastFourCardDigits());
        Assert.Equal(paymentResponse.ExpiryYear, ppr.ExpiryYear);
        Assert.Equal(paymentResponse.Amount, ppr.Amount);
        Assert.Equal(paymentResponse.Currency, ppr.Currency);
    }

    [Fact]
    public async Task PostPaymentRequest_DeclinedSuccessfully2()
    {
        PostPaymentRequest ppr = new()
        {
            CardNumber = "2222405343248112",
            ExpiryYear = 2026,
            ExpiryMonth = 1,
            Amount = 60000,
            Currency = "USD",
            Cvv = "456"
        };

        var paymentsRepository = new PaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        var response = await client.PostAsJsonAsync("/api/Payments", ppr);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(paymentResponse.Status, Enums.PaymentStatus.Declined);
        Assert.Equal(paymentResponse.CardNumberLastFour, ppr.GetLastFourCardDigits());
        Assert.Equal(paymentResponse.ExpiryYear, ppr.ExpiryYear);
        Assert.Equal(paymentResponse.Amount, ppr.Amount);
        Assert.Equal(paymentResponse.Currency, ppr.Currency);
    }

    [Fact]
    public async Task PostPaymentRequest_AuthorizedSuccessfully()
    {
        PostPaymentRequest ppr = new()
        {
            CardNumber = "2222405343248877",
            ExpiryYear = 2025,
            ExpiryMonth = 4,
            Amount = 100,
            Currency = "GBP",
            Cvv = "123"
        };

        var paymentsRepository = new PaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        var response = await client.PostAsJsonAsync("/api/Payments", ppr);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(paymentResponse.Status, Enums.PaymentStatus.Authorized);
    }
}