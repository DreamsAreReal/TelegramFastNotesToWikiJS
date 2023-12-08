using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramFastNotesToWikiJS.Domain.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Abstractions;

namespace TelegramFastNotesToWikiJS.Infrastructure.Telegram;

internal class TelegramMessageReceiver(
    IOptions<TelegramConfiguration> configuration,
    ITelegramUpdateHandler telegramUpdateHandler,
    ILogger<TelegramMessageReceiver> logger
) : IMessageReceiver
{
    public event Func<MessageData, Task> OnMessageReceived;

    private readonly UpdateType[] _allowedUpdates = { UpdateType.Message, };
    private readonly TelegramBotClient _bot = new(configuration.Value.Token);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isStarted;

    public void Dispose()
    {
        telegramUpdateHandler.OnMessageReceived -= OnMessageReceived;
        _cancellationTokenSource.Dispose();
    }

    public async Task Start()
    {
        if (_isStarted)
            throw new InvalidOperationException("The bot is already started.");

        if (!await _bot.TestApiAsync(_cancellationTokenSource.Token))
            throw new InvalidOperationException("The API test was unsuccessful.");

        _bot.StartReceiving(
            telegramUpdateHandler, new() { AllowedUpdates = _allowedUpdates, }, _cancellationTokenSource.Token
        );

        telegramUpdateHandler.OnMessageReceived += OnMessageReceived;
        _isStarted = true;
        string allowedUpdatesStr = string.Join(",", _allowedUpdates.Select(x => x.ToString()));
        logger.LogInformation("The bot has started. Allowed updates: {AllowedUpdates}", allowedUpdatesStr);
    }
}