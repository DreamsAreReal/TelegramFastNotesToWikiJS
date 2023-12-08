namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

public record MessageData(string? Message, IEnumerable<string>? PhotosInBase64)
{
    public DateTime ReceivedAt { get; } = DateTime.UtcNow;
}