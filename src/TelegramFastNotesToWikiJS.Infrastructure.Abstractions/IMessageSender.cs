using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions;

public interface IMessageSender
{
    Task SendMessageAsync(MessageToSend message, CancellationToken cancellationToken);
}