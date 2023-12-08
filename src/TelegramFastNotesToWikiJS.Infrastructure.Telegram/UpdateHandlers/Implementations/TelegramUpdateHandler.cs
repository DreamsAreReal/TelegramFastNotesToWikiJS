using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFastNotesToWikiJS.Domain.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Abstractions;
using File = Telegram.Bot.Types.File;

namespace TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Implementations;

internal class TelegramUpdateHandler(
    IOptions<TelegramConfiguration> configuration,
    ILogger<TelegramUpdateHandler> logger
) : ITelegramUpdateHandler
{
    public event Func<MessageData, Task>? OnMessageReceived;

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
        string? messageText = update.Message.Text;
        string[] photosInBase64 = Array.Empty<string>();

        if (update.Message.Photo is not null)
            photosInBase64 = (await GetPhotoContentAsync(botClient, update.Message.Photo)).ToArray();

        await OnMessageReceived.Invoke(
            new(
                string.IsNullOrWhiteSpace(messageText) ? null : messageText,
                photosInBase64.Length == 0 ? null : photosInBase64
            )
        );

        timer.Stop();

        logger.LogInformation(
            "Received message. Time elapsed {ElapsedMilliseconds} ms", timer.ElapsedMilliseconds
        );
    }

    private async Task<IEnumerable<string>> GetPhotoContentAsync(ITelegramBotClient botClient,
                                                                 IEnumerable<PhotoSize> photos
    )
    {
        List<string> photoContentList = [];

        foreach (PhotoSize photo in photos)
        {
            string? base64Content = await GetPhotoContentInBase64Async(botClient, photo.FileId);

            if (!string.IsNullOrWhiteSpace(base64Content))
                photoContentList.Add(base64Content);
        }

        return photoContentList;
    }

    private async Task<string?> GetPhotoContentInBase64Async(ITelegramBotClient botClient, string fileId)
    {
        File file = await botClient.GetFileAsync(fileId);

        if (string.IsNullOrWhiteSpace(file.FilePath))
            return null;

        using MemoryStream memoryStream = new();
        await botClient.DownloadFileAsync(file.FilePath, memoryStream);
        memoryStream.Position = 0;
        byte[] bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }
}