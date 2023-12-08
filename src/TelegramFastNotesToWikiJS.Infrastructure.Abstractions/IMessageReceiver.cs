using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.Abstractions;

public interface IMessageReceiver : IDisposable
{
    event Func<MessageData, Task> OnMessageReceived;
    Task Start();
}