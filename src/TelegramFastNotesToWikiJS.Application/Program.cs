using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramFastNotesToWikiJS.Application;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs;

IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("appsettings.json", true, true)
                                                          .AddUserSecrets(
                                                              Assembly.GetExecutingAssembly(), true
                                                          );

IConfiguration config = builder.Build();
IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddTelegram(config);
serviceCollection.AddWikiJs(config);

serviceCollection.AddLogging(
    x => x.AddSimpleConsole(
        options =>
        {
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        }
    )
);

serviceCollection.Configure<EnvironmentConfiguration>(config.GetSection(nameof(EnvironmentConfiguration)));
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
ILogger<Startup> logger = serviceProvider.GetService<ILogger<Startup>>()!;
await new Startup(logger).Start(serviceProvider).ConfigureAwait(false);