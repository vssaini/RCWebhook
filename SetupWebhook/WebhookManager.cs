using RingCentral;
using System.Text;
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

    private const string MessageFilter = "/restapi/v1.0/account/~/extension/~/message-store/instant?type=SMS";

    /// <summary>
    /// Asynchronously creates a webhook subscription for receiving voicemail notifications from RingCentral.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// Builds a subscription request for voicemail events, sends it to the RingCentral API,
    /// and logs the response details to the console. In case of failure, logs the error message.
    /// </remarks>
    public async Task CreateSubscriptionForNotificationAsync()
    {
        try
        {
            var request = BuildSubscriptionRequestForVoiceMail(3600);
            //var request = BuildSubscriptionRequestForMessage();
            var response = await client.Restapi().Subscription().Post(request);

            ConsolePrinter.Success("Subscription created successfully.");
            LogSubscriptionResponse(response);
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Subscription failed: " + ex.Message);
        }
    }

    /// <summary>
    /// Logs the subscription response in a structured format.
    /// </summary>
    /// <param name="response">The response object containing subscription details.</param>
    private static void LogSubscriptionResponse(SubscriptionInfo response)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"Subscription ID: {response.id}, Subscription URI: {response.uri}, Event Filters: {string.Join(", ", response.eventFilters)}");
        sb.AppendLine($"Expiration Time: {response.expirationTime}, Expires In: {response.expiresIn} seconds");
        sb.AppendLine($"Subscription Status: {response.status}, Creation Time: {response.creationTime}");

        // Adding Webhook Delivery Details if available
        if (response.deliveryMode != null)
        {
            sb.AppendLine("Webhook Delivery Mode:");
            sb.AppendLine($" • Transport Type: {response.deliveryMode.transportType}");
            sb.AppendLine($" • Address: {response.deliveryMode.address}");
            if (!string.IsNullOrWhiteSpace(response.deliveryMode.secretKey))
                sb.AppendLine($" • Secret Key: {response.deliveryMode.secretKey}");
        }
        
        ConsolePrinter.Info(sb.ToString());
    }

    /// <summary>
    /// Builds a <see cref="CreateSubscriptionRequest"/> object for receiving voicemail notifications via WebHook.
    /// </summary>
    /// <param name="subscriptionExpireInSeconds">
    /// The time in seconds for which the subscription remains active before expiration. 
    /// For example, use 3600 for a 1-hour subscription.
    /// </param>
    /// <returns>
    /// A configured <see cref="CreateSubscriptionRequest"/> object including event filters, 
    /// delivery mode, and expiration settings for voicemail WebHook notifications.
    /// </returns>
    /// <remarks>
    /// Reference: https://developers.ringcentral.com/guide/notifications/webhooks/receiving
    /// </remarks>
    private CreateSubscriptionRequest BuildSubscriptionRequestForVoiceMail(long subscriptionExpireInSeconds)
    {
        var devDefinedToken = TokenGenerator.GenerateVerificationToken("dev");
        ConsolePrinter.Info("Developer defined verification token (to be used in Azure Function as WebhookSecret): " + devDefinedToken);

        return new CreateSubscriptionRequest
        {
            eventFilters = [VoiceMailFilter],
            deliveryMode = new NotificationDeliveryModeRequest
            {
                transportType = "WebHook",
                address = deliveryAddress,
                verificationToken = devDefinedToken
            },
            expiresIn = subscriptionExpireInSeconds
        };
    }

    /// <summary>
    /// Builds a subscription request for message notifications.
    /// </summary>
    /// <returns>A populated <see cref="CreateSubscriptionRequest"/> object.</returns>
    // ReSharper disable once UnusedMember.Local
    private CreateSubscriptionRequest BuildSubscriptionRequestForMessage()
    {
        return new CreateSubscriptionRequest
        {
            eventFilters = [MessageFilter],
            deliveryMode = new NotificationDeliveryModeRequest
            {
                transportType = "WebHook",
                address = deliveryAddress
            },
            expiresIn = 3600
        };
    }

    /// <summary>
    /// Fetch all subscriptions existing in RingCentral.
    /// </summary>
    /// <returns></returns>
    public async Task FetchAllSubscriptionsAsync()
    {
        try
        {
            var response = await client.Restapi().Subscription().List();
            if (response.records.Length == 0)
            {
                ConsolePrinter.Warning("No subscription exist in RingCentral.");
                return;
            }

            foreach (var record in response.records)
            {
                ConsolePrinter.Debug(JsonSerializer.Serialize(record));
            }
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error("Failed to read subscriptions: " + ex.Message);
        }
    }

    /// <summary>
    /// Delete all subscriptions.
    /// </summary>
    public async Task DeleteAllSubscriptionsAsync()
    {
        try
        {
            var response = await client.Restapi().Subscription().List();
            if (response.records.Length == 0)
            {
                ConsolePrinter.Warning("No subscription exist in RingCentral.");
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
            ConsolePrinter.Error("Failed to delete subscriptions: " + ex.Message);
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
