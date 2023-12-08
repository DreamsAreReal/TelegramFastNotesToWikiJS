using Telegram.Bot.Polling;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Abstractions;

internal interface ITelegramUpdateHandler : IUpdateHandler
{
    event Func<ReceivedMessage, Task> OnMessageReceived;
}