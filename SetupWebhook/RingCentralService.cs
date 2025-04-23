using RingCentral;

namespace SetupWebhook;

/// <summary>
/// Provides services to interact with the RingCentral API, including initialization
/// of the client and authorization.
/// </summary>
public class RingCentralService
{
    /// <summary>
    /// Gets the RestClient instance used to communicate with the RingCentral API.
    /// </summary>
    public RestClient? Client { get; private set; }

    /// <summary>
    /// Gets the delivery address for the webhook, constructed from the environment variable.
    /// </summary>
    public string? DeliveryAddress { get; private set; }

    /// <summary>
    /// Initializes the RingCentralService by setting up the RestClient and
    /// authorizing using environment variables.
    /// </summary>
    /// <exception cref="Exception">Thrown when authorization fails.</exception>
    public async Task InitializeAsync()
    {
        DeliveryAddress = Environment.GetEnvironmentVariable("WEBHOOK_DELIVERY_ADDRESS") + "/webhook";
        ConsolePrinter.Info("Webhook delivery address: " + DeliveryAddress);

        Client = new RestClient(
            Environment.GetEnvironmentVariable("RC_APP_CLIENT_ID"),
            Environment.GetEnvironmentVariable("RC_APP_CLIENT_SECRET"),
            Environment.GetEnvironmentVariable("RC_SERVER_URL"));

        try
        {
            await Client.Authorize(Environment.GetEnvironmentVariable("RC_USER_JWT"));
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Authorization failed: " + ex.Message);
            throw;
        }
    }
}
