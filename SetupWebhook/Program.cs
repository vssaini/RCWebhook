using dotenv.net;

namespace SetupWebhook;

class Program
{
    static async Task Main()
    {
        // Ref - https://github.com/bolorundurowb/dotenv.net
        // Load environment variables from .env file
        DotEnv.Load();

        var service = new RingCentralService();
        await service.InitializeAsync();

        if (service.Client == null)
        {
            ConsolePrinter.Error("Failed to initialize RingCentral client.");
            Console.ReadKey();
            return;
        }

        var manager = new WebhookManager(service.Client, service.DeliveryAddress);

        await manager.CreateSubscriptionForNotificationAsync();
        //await manager.FetchAllSubscriptionsAsync();
        //await manager.DeleteAllSubscriptionsAsync();
    }
}