using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.UpdateHandlers.Implementations;

namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTelegram(this IServiceCollection serviceCollection,
                                                 IConfiguration configuration
    )
    {
        serviceCollection.Configure<TelegramConfiguration>(
            configuration.GetSection(nameof(TelegramConfiguration))
        );

        serviceCollection.AddSingleton<TelegramClient>();
        serviceCollection.AddSingleton<IMessageReceiver, TelegramMessageReceiver>();
        serviceCollection.AddSingleton<IMessageSender, TelegramMessageSender>();
        serviceCollection.AddSingleton<ITelegramUpdateHandler, TelegramUpdateHandler>();
        serviceCollection.AddTransient<TelegramUtilities>();
        return serviceCollection;
    }
}