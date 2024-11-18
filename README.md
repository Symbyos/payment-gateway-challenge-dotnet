## Decisions

### Payment repository

The current solution uses a `ConcurrentDictionary` to store the records of payment responses, as it provides a constant time to retrieval.
The payment repository is a singleton service that can be accessed by multiple threads at the same time; to handle possible concurrency issues the solution is using a `ConcurrentDictionary`.

### Immutable requests and responses

The solution stores the bank responses into a repository. As we want to keep a consistent state during operations and between queries, requests and responses are immutable.
To achieve this feature, the solution used the `record` type and `init` setters.

### Payment status

There is an ambiguity between the requirements https://github.com/cko-recruitment/#requirements and the data models for payment responses and payment details https://github.com/cko-recruitment/#processing-a-payment.
The requirements requested 3 different status (`Authorized`, `Declined` and `Rejected`) to a processed payment whereas the data models descriptions stipulated only 2 (`Authorized` and `Declined`). 
I have kept the third state `Rejected`, as it can help the merchant to understand if there was an issue in its usage of the API.

### Acquiring bank authorization codes

The current solution doesn't store the authorization codes as it is not a requirement, but it could be needed in the future for compliance and transaction reconciliation purposes.

### Async/await model - Don't block, await instead

Following best practices as documented in https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0, the solution uses the `async`/`await` model to efficiently manage its resources.

### Handling resources

To avoid resource exhaustion problems, the solution uses a `HttpClientFactory` injected at the application start through dependency injection to manage its pool of `HttpClient`.
It follows best practices as described in https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#httpclient-and-lifetime-management.

### Health check

A health check service has been added to the solution with a routing to `/healthz` to have a simple check that the API is healthy.

### Test

The tests in `PaymentsControllerTests.cs` are using a mocked `HttpClientFactory` to simulate responses from the bank API. It allows testing the solution behaviour without having to spin up a bank simulator.
