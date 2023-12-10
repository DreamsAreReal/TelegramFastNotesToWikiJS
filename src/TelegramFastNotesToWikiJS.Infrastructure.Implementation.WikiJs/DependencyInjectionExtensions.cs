using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs;

namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddWikiJs(this IServiceCollection serviceCollection,
                                               IConfiguration configuration
    )
    {
        serviceCollection.Configure<WikiJsConfiguration>(
            configuration.GetSection(nameof(WikiJsConfiguration))
        );

        serviceCollection.Configure<WikiJsPageTemplateConfiguration>(
            configuration.GetSection(nameof(WikiJsPageTemplateConfiguration))
        );

        serviceCollection.AddSingleton<WikiJsGraphQlWrapper>();
        serviceCollection.AddSingleton<IWikiJsApi, WikiJsApi>();
        return serviceCollection;
    }
}