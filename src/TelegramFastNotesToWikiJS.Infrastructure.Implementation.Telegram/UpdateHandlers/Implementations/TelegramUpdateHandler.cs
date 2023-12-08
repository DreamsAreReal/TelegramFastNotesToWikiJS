using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFastNotesToWikiJS.Domain.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Abstractions;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Implementations;

internal class TelegramUpdateHandler(
    IOptions<TelegramConfiguration> configuration,
    ILogger<TelegramUpdateHandler> logger,
    TelegramUtilities telegramUtilities
) : ITelegramUpdateHandler
{
    public event Func<ReceivedMessage, Task>? OnMessageReceived;

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient,
                                        Exception exception,
                                        CancellationToken cancellationToken
    )
    {
        throw exception;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient,
                                        Update update,
                                        CancellationToken cancellationToken
    )
    {
        if (OnMessageReceived is null)
            throw new ArgumentException("MessageReceived event is not handled.");

        if (update.Message is null)
        {
            logger.LogError("Received Update event {UpdateId}. Message is null", update.Id);
            return;
        }

        if (update.Message.Chat.Id != configuration.Value.OwnerId)
        {
            logger.LogError(
                "Received chat ID ({ChatId}) does not match the expected owner ID {ValueOwnerId}",
                update.Message.Chat.Id, configuration.Value.OwnerId
            );

            return;
        }

        Stopwatch timer = Stopwatch.StartNew();
        string? text = telegramUtilities.ExtractTextFromUpdate(update.Message);
        string? photo = await telegramUtilities.GetPhotoAsync(update.Message, botClient);

        if (string.IsNullOrWhiteSpace(text) &&
            string.IsNullOrWhiteSpace(photo))
        {
            timer.Stop();

            logger.LogWarning(
                "Both text and photo cannot be null or whitespace. Time elapsed {ElapsedMilliseconds} ms",
                timer.ElapsedMilliseconds
            );

            return;
        }

        await OnMessageReceived.Invoke(
            new(
                telegramUtilities.GetGroupOrMessageId(update.Message),
                telegramUtilities.ExtractTextFromUpdate(update.Message),
                await telegramUtilities.GetPhotoAsync(update.Message, botClient),
                telegramUtilities.HasMediaGroup(update.Message)
            )
        );

        timer.Stop();

        logger.LogInformation(
            "Received message. Time elapsed {ElapsedMilliseconds} ms", timer.ElapsedMilliseconds
        );
    }
}