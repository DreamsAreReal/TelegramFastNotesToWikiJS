using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramFastNotesToWikiJS.Domain.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Abstractions;

namespace TelegramFastNotesToWikiJS.Infrastructure.Telegram;

internal class TelegramMessageReceiver : IMessageReceiver
{
    public event Func<MessageData, Task>? OnMessageReceived;

    private readonly UpdateType[] _allowedUpdates = { UpdateType.Message, };
    private readonly TelegramBotClient _bot;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IOptions<TelegramConfiguration> _configuration;
    private bool _isStarted;
    private readonly ILogger<TelegramMessageReceiver> _logger;
    private readonly ITelegramUpdateHandler _telegramUpdateHandler;

    public TelegramMessageReceiver(IOptions<TelegramConfiguration> configuration,
                                   ITelegramUpdateHandler telegramUpdateHandler,
                                   ILogger<TelegramMessageReceiver> logger
    )
    {
        _configuration = configuration;
        _telegramUpdateHandler = telegramUpdateHandler;
        _logger = logger;
        _bot = new(_configuration.Value.Token);
    }

    public void Dispose()
    {
        _telegramUpdateHandler.OnMessageReceived -= OnMessageReceived;
        _cancellationTokenSource.Dispose();
    }

    public async Task Start()
    {
        if (_isStarted)
            throw new InvalidOperationException("The bot is already started.");

        if (!await _bot.TestApiAsync(_cancellationTokenSource.Token))
            throw new InvalidOperationException("The API token test was unsuccessful.");

        _bot.StartReceiving(
            _telegramUpdateHandler, new() { AllowedUpdates = _allowedUpdates, },
            _cancellationTokenSource.Token
        );

        _telegramUpdateHandler.OnMessageReceived += OnMessageReceived;
        _isStarted = true;
        string allowedUpdatesStr = string.Join(",", _allowedUpdates.Select(x => x.ToString()));
        _logger.LogInformation("The bot has started. Allowed updates: {AllowedUpdates}", allowedUpdatesStr);
    }
}