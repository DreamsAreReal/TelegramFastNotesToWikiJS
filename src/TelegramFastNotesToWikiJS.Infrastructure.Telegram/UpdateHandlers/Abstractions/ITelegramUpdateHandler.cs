using Telegram.Bot.Polling;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Abstractions;

internal interface ITelegramUpdateHandler : IUpdateHandler
{
    event Func<MessageData, Task> OnMessageReceived;
}