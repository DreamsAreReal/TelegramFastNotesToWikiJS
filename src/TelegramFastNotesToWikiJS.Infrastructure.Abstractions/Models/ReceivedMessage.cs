namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

public record ReceivedMessage(string messageId, string? Message, string? PhotoInBase64, bool isBulk)
{
    public DateTime ReceivedAt { get; } = DateTime.UtcNow;
}