using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

internal class TelegramUtilities
{
    private async Task<string?> DownloadAndConvertFileAsync(ITelegramBotClient botClient, File file)
    {
        if (string.IsNullOrWhiteSpace(file.FilePath))
            return null;

        using MemoryStream memoryStream = new();
        await botClient.DownloadFileAsync(file.FilePath, memoryStream).ConfigureAwait(false);
        memoryStream.Position = 0;
        byte[] bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }

    private async Task<string?> DownloadFileInBase64Async(ITelegramBotClient botClient, string fileId)
    {
        File file = await botClient.GetFileAsync(fileId).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(file.FilePath))
            return null;

        return await DownloadAndConvertFileAsync(botClient, file).ConfigureAwait(false);
    }

    internal string? ExtractTextFromUpdate(Message message)
    {
        return string.IsNullOrWhiteSpace(message.Text) ? message.Caption : message.Text;
    }

    internal string GetGroupOrMessageId(Message message)
    {
        return !string.IsNullOrWhiteSpace(message.MediaGroupId) ?
                   message.MediaGroupId :
                   message.MessageId.ToString();
    }

    internal async Task<string?> GetPhotoAsync(Message message, ITelegramBotClient botClient)
    {
        if (message.Photo is null)
            return null;

        string? photoId = message.Photo.Last().FileId;

        if (string.IsNullOrWhiteSpace(photoId))
            return null;

        string? data = await DownloadFileInBase64Async(botClient, photoId).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(data))
            return null;

        StringBuilder sb = new(data);
        sb.Insert(0, "data:image/png;base64,");
        return sb.ToString();
    }

    internal bool HasMediaGroup(Message message)
    {
        return !string.IsNullOrWhiteSpace(message.MediaGroupId);
    }
}