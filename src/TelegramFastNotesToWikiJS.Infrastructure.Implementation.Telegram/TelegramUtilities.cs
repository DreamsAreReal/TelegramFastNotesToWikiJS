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
        await botClient.DownloadFileAsync(file.FilePath, memoryStream);
        memoryStream.Position = 0;
        byte[] bytes = memoryStream.ToArray();
        return Convert.ToBase64String(bytes);
    }

    private async Task<string?> DownloadFileInBase64Async(ITelegramBotClient botClient, string fileId)
    {
        File file = await botClient.GetFileAsync(fileId);

        if (string.IsNullOrWhiteSpace(file.FilePath))
            return null;

        return await DownloadAndConvertFileAsync(botClient, file);
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

        if (!string.IsNullOrWhiteSpace(photoId))
            return await DownloadFileInBase64Async(botClient, photoId);

        return null;
    }

    internal bool HasMediaGroup(Message message)
    {
        return !string.IsNullOrWhiteSpace(message.MediaGroupId);
    }
}