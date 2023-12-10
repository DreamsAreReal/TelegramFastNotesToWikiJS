using Telegram.Bot;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

internal class TelegramMessageSender : IMessageSender
{
    private readonly TelegramClient _client;

    public TelegramMessageSender(TelegramClient client)
    {
        _client = client;
    }

    public async Task SendMessageAsync(MessageToSend message, CancellationToken cancellationToken)
    {
        await _client.Bot.SendTextMessageAsync(
                         _client.Configuration.OwnerId, $"{message.Text}",
                         cancellationToken: cancellationToken
                     )
                     .ConfigureAwait(false);
    }
}