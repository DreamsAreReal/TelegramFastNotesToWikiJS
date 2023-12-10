using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Abstractions;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

internal class TelegramMessageReceiver : IMessageReceiver
{
    public event Func<ReceivedMessage, Task>? OnMessageReceivedAsync;

    private readonly UpdateType[] _allowedUpdates = { UpdateType.Message, };
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isStarted;
    private readonly ILogger<TelegramMessageReceiver> _logger;
    private readonly TelegramClient _telegramClient;
    private readonly ITelegramUpdateHandler _telegramUpdateHandler;

    public TelegramMessageReceiver(ITelegramUpdateHandler telegramUpdateHandler,
                                   ILogger<TelegramMessageReceiver> logger,
                                   TelegramClient telegramClient
    )
    {
        _telegramUpdateHandler = telegramUpdateHandler;
        _logger = logger;
        _telegramClient = telegramClient;
    }

    public void Dispose()
    {
        _telegramUpdateHandler.OnMessageReceivedAsync -= OnMessageReceivedAsync;
        _cancellationTokenSource.Dispose();
    }

    public async Task Start()
    {
        if (_isStarted)
            throw new InvalidOperationException("The bot is already started.");

        if (!await _telegramClient.Bot.TestApiAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
            throw new InvalidOperationException("The API token test was unsuccessful.");

        _telegramClient.Bot.StartReceiving(
            _telegramUpdateHandler, new() { AllowedUpdates = _allowedUpdates, },
            _cancellationTokenSource.Token
        );

        _telegramUpdateHandler.OnMessageReceivedAsync += OnMessageReceivedAsync;
        _isStarted = true;
        string allowedUpdatesStr = string.Join(",", _allowedUpdates.Select(x => x.ToString()));
        _logger.LogInformation("The bot has started. Allowed updates: {AllowedUpdates}", allowedUpdatesStr);
    }
}