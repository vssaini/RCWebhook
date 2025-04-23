using RingCentral;

namespace SetupWebhook;

public class RingCentralService
{
    public RestClient? Client { get; private set; }
    public string? DeliveryAddress { get; private set; }

    public async Task InitializeAsync()
    {
        DeliveryAddress = Environment.GetEnvironmentVariable("WEBHOOK_DELIVERY_ADDRESS") + "/webhook";
        ConsolePrinter.Info("Delivery address: " + DeliveryAddress);
        
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