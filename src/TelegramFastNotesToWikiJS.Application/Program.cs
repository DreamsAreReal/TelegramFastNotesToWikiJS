using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramFastNotesToWikiJS.Application;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram;

IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("appsettings.json", true, true)
                                                          .AddUserSecrets(
                                                              Assembly.GetExecutingAssembly(), true
                                                          );

IConfiguration config = builder.Build();

IServiceCollection serviceCollection = new ServiceCollection().AddLogging(
                                                                  x => x.AddSimpleConsole(
                                                                      options =>
                                                                      {
                                                                          options.TimestampFormat =
                                                                              "[yyyy-MM-dd HH:mm:ss] ";
                                                                      }
                                                                  )
                                                              )
                                                              .AddTelegramMessageReceiver(config);

serviceCollection.Configure<EnvironmentConfiguration>(config.GetSection(nameof(EnvironmentConfiguration)));
ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
await new Startup().Start(serviceProvider);