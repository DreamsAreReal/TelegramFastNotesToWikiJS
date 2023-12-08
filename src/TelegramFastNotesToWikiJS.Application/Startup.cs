using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
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
        IMessageSender[] messageSenders = serviceProvider.GetServices<IMessageSender>().ToArray();

        try
        {
            foreach (IMessageReceiver messageReceiver in messageReceivers)
            {
                messageReceiver.OnMessageReceived += async data =>
                {
                    foreach (IMessageSender messageSender in messageSenders)
                        await messageSender.SendMessage(new(data.messageId), CancellationToken.None).ConfigureAwait(false);
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