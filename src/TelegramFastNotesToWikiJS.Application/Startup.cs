using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramFastNotesToWikiJS.Application.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using Environment = TelegramFastNotesToWikiJS.Application.Configurations.Environment;

namespace TelegramFastNotesToWikiJS.Application;

public class Startup(ILogger<Startup> logger)
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public async Task Start(IServiceProvider serviceProvider)
    {
        Environment environment = GetEnvironmentOptions(serviceProvider).Value.Environment;
        IMessageReceiver[] messageReceivers = GetServices<IMessageReceiver>(serviceProvider);
        IMessageSender[] messageSenders = GetServices<IMessageSender>(serviceProvider);
        IWikiJsApi wikiJsApi = GetService<IWikiJsApi>(serviceProvider);

        try
        {
            await StartReceivingMessages(messageReceivers, messageSenders, wikiJsApi);
        }
        catch (Exception exception)
        {
            if (environment is Environment.Development)
                throw;
            else
                logger.LogCritical("Exception {Exception}", exception);
        }
        finally
        {
            await DisposeResources(environment, messageReceivers);
        }
    }

    private static T GetService<T>(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<T>()!;
    }

    private static T[] GetServices<T>(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<T>().ToArray();
    }

    private async Task DisposeResources(Environment environment, IMessageReceiver[] messageReceivers)
    {
        await _cancellationTokenSource.CancelAsync();

        if (environment is Environment.Development)
            foreach (IMessageReceiver messageReceiver in messageReceivers)
                messageReceiver.Dispose();
    }

    private IOptions<EnvironmentConfiguration> GetEnvironmentOptions(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<IOptions<EnvironmentConfiguration>>()!;
    }

    private async Task ProcessReceivedMessage(ReceivedMessage message,
                                              IWikiJsApi wikiJsApi,
                                              IMessageSender[] messageSenders
    )
    {
        bool result = await wikiJsApi.TryCreateOrUpdateAsync(message, _cancellationTokenSource.Token);

        MessageToSend messageToSend = new(
            $"{message.MessageId}\n{(result ? "Synced \u2705" : "Not synced \u274c")}"
        );

        foreach (IMessageSender messageSender in messageSenders)
            await messageSender.SendMessageAsync(messageToSend, _cancellationTokenSource.Token);
    }

    private async Task RunInLoop()
    {
        while (true)
            await Task.Delay(TimeSpan.FromDays(1));
    }

    private async Task StartReceivingMessages(IMessageReceiver[] messageReceivers,
                                              IMessageSender[] messageSenders,
                                              IWikiJsApi wikiJsApi
    )
    {
        foreach (IMessageReceiver messageReceiver in messageReceivers)
        {
            messageReceiver.OnMessageReceivedAsync += message =>
            {
                return ProcessReceivedMessage(message, wikiJsApi, messageSenders);
            };

            await messageReceiver.Start();
        }

        await RunInLoop();
    }
}