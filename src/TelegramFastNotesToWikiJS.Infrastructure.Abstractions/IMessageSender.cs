using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions;

public interface IMessageSender
{
    Task SendMessage(MessageToSend message, CancellationToken cancellationToken);
}