namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

public record ReceivedMessage(string MessageId, string? Message, string? PhotoInBase64, bool IsBulk);