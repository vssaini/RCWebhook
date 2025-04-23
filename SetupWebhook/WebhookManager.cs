using RingCentral;
using System.Text.Json;

namespace SetupWebhook;

/// <summary>
/// Manages RingCentral webhook subscriptions for voicemail notifications.
/// </summary>
/// <param name="client">An instance of the RingCentral RestClient.</param>
/// <param name="deliveryAddress">The webhook delivery URL where notifications will be sent.</param>
public class WebhookManager(RestClient client, string? deliveryAddress)
{
    // Ref - https://developers.ringcentral.com/api-reference/Voicemail-Message-Event

    /// <summary>
    /// Event filter for voicemail messages.
    /// </summary>
    private const string VoiceMailFilter = "/restapi/v1.0/account/~/extension/~/voicemail";

    /// <summary>
    /// Subscribes to voicemail notification events using the configured delivery address.
    /// </summary>
    public async Task SubscribeForNotificationAsync()
    {
        try
        {
            var request = BuildSubscriptionRequest();
            var response = await client.Restapi().Subscription().Post(request);

            ConsolePrinter.Success("Subscription Id: " + response.id);
            ConsolePrinter.Info("Ready to receive incoming Voicemail via WebHook.");
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Subscription failed: " + ex.Message);
        }
    }

    /// <summary>
    /// Builds a subscription request for voicemail notifications.
    /// </summary>
    /// <returns>A populated <see cref="CreateSubscriptionRequest"/> object.</returns>
    private CreateSubscriptionRequest BuildSubscriptionRequest()
    {
        return new CreateSubscriptionRequest
        {
            eventFilters = [VoiceMailFilter],
            deliveryMode = new NotificationDeliveryModeRequest
            {
                transportType = "WebHook",
                address = deliveryAddress
            },
            expiresIn = 3600
        };
    }

    /// <summary>
    /// Lists all current subscriptions and deletes them one by one.
    /// </summary>
    public async Task ReadAndDeleteAllSubscriptionsAsync()
    {
        try
        {
            var response = await client.Restapi().Subscription().List();
            if (response.records.Length == 0)
            {
                ConsolePrinter.Warning("No subscription.");
                return;
            }

            foreach (var record in response.records)
            {
                ConsolePrinter.Debug(JsonSerializer.Serialize(record));
                await DeleteSubscriptionAsync(record.id);
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to read subscriptions: " + ex.Message);
        }
    }

    /// <summary>
    /// Deletes a specific subscription by its ID.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to delete.</param>
    public async Task DeleteSubscriptionAsync(string subscriptionId)
    {
        try
        {
            await client.Restapi().Subscription(subscriptionId).Delete();
            ConsolePrinter.Success("Subscription " + subscriptionId + " deleted.");
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to delete subscription: " + ex.Message);
        }
    }
}
