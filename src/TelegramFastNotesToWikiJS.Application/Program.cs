using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramFastNotesToWikiJS.Infrastructure.Telegram;

IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddUserSecrets(Assembly.GetExecutingAssembly())
                                                          .AddJsonFile("appsettings.json", true, true);

IConfiguration config = builder.Build();

IServiceCollection serviceCollection =
    new ServiceCollection().AddLogging().AddTelegramMessageReceiver(config);

ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();