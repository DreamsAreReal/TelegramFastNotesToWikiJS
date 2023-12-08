using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramFastNotesToWikiJS.Application;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram;

IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("appsettings.json", true, true)
                                                          .AddUserSecrets(
                                                              Assembly.GetExecutingAssembly(), true
                                                          );

IConfiguration config = builder.Build();

IServiceCollection serviceCollection =
    new ServiceCollection().AddLogging().AddTelegramMessageReceiver(config);

serviceCollection.Configure<EnvironmentConfiguration>(config.GetSection(nameof(EnvironmentConfiguration)));
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
await new Startup().Start(serviceProvider);