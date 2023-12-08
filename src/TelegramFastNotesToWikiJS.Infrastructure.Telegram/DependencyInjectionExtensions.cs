using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramFastNotesToWikiJS.Domain.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram.UpdateHandlers.Implementations;

namespace TelegramFastNotesToWikiJS.Infrastructure.Telegram;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTelegramMessageReceiver(this IServiceCollection serviceCollection,
                                                                IConfiguration configuration
    )
    {
        serviceCollection.AddOptions<TelegramConfiguration>(
            configuration.GetSection(nameof(TelegramConfiguration)).ToString()
        );

        serviceCollection.AddSingleton<IMessageReceiver, TelegramMessageReceiver>();
        serviceCollection.AddSingleton<ITelegramUpdateHandler, TelegramUpdateHandler>();
        return serviceCollection;
    }
}