using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.Configurations;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

internal class TelegramClient
{
    public TelegramClient(IOptions<TelegramConfiguration> configuration)
    {
        Configuration = configuration.Value;
        Bot = new(Configuration.Token);
    }

    internal TelegramBotClient Bot { get; }
    internal TelegramConfiguration Configuration { get; }
}