using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using Environment = TelegramFastNotesToWikiJS.Application.Configurations.Environment;

namespace TelegramFastNotesToWikiJS.Application;

public class Startup
{
    public async Task Start(IServiceProvider serviceProvider)
    {
        IOptions<EnvironmentConfiguration>? environmentOptions =
            serviceProvider.GetService<IOptions<EnvironmentConfiguration>>();

        Environment environment = environmentOptions is null ?
                                      Environment.Development :
                                      environmentOptions.Value.Environment;

        IMessageReceiver[] messageReceivers = serviceProvider.GetServices<IMessageReceiver>().ToArray();

        try
        {
            foreach (IMessageReceiver messageReceiver in messageReceivers)
            {
                messageReceiver.OnMessageReceived += data =>
                {
                    MessageData s = data;
                    return Task.CompletedTask;
                };

                await messageReceiver.Start();
            }

            while (true)
                await Task.Delay(100);
        }
        catch
        {
            if (environment is Environment.Development)
                throw;
        }
        finally
        {
            if (environment is Environment.Development)
                foreach (IMessageReceiver messageReceiver in messageReceivers)
                    messageReceiver.Dispose();
        }
    }
}