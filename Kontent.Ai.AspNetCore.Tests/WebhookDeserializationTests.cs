using System;
using System.Text.Json;
using Xunit;
using Kontent.Ai.AspNetCore.Webhooks.Models;
using System.IO;

namespace Kontent.Ai.AspNetCore.Tests
{
    public class WebhookNotificationTests
    {
        [Fact]
        public void WebhookNotification_Deserialization_PopulatesPropertiesCorrectly()
        {
            string notificationFile = Path.Combine(Environment.CurrentDirectory, "Data", "PublishWebhookActionBody.json");
            string jsonPayload = File.ReadAllText(notificationFile);

            var notification = JsonSerializer.Deserialize<WebhookNotification>(jsonPayload);

            Assert.NotNull(notification);
            Assert.NotNull(notification.Notifications);
            Assert.Single(notification.Notifications);
            
            var webhookModel = notification.Notifications[0];
            Assert.NotNull(webhookModel.Data);
            Assert.NotNull(webhookModel.Message);

            var data = webhookModel.Data.System;
            Assert.Equal(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), data.Id);
            Assert.Equal("Test Item", data.Name);
            Assert.Equal("default_workflow", data.Workflow);

            var message = webhookModel.Message;
            Assert.Equal(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), message.EnvironmentId);
            Assert.Equal("content_item_variant", message.ObjectType);
            Assert.Equal("published", message.DeliverySlot);
        }
    }
}
