using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions;

public interface IWikiJsApi
{
    Task<bool> TryCreateOrUpdateAsync(ReceivedMessage message, CancellationToken cancellationToken);
}