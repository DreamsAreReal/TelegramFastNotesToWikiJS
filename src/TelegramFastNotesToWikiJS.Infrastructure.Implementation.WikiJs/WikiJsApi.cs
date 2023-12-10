using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions;
using TelegramFastNotesToWikiJS.Infrastructure.Abstractions.Models;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs;

public class WikiJsApi : IWikiJsApi
{
    private readonly WikiJsGraphQlWrapper _apiWrapper;
    private readonly ILogger<WikiJsApi> _logger;
    private readonly IOptions<WikiJsPageTemplateConfiguration> _pageTemplateOptions;

    public WikiJsApi(IOptions<WikiJsPageTemplateConfiguration> pageTemplateOptions,
                     WikiJsGraphQlWrapper apiWrapper,
                     ILogger<WikiJsApi> logger
    )
    {
        _pageTemplateOptions = pageTemplateOptions;
        _apiWrapper = apiWrapper;
        _logger = logger;
    }

    public async Task<bool> TryCreateOrUpdateAsync(ReceivedMessage message,
                                                   CancellationToken cancellationToken
    )
    {
        if (!message.IsBulk)
            return await CreatePageAsync(message, GenerateContent(message), cancellationToken);

        Search.Page? searchResult = (await _apiWrapper.SearchPagesAsync(
                                         $"{_pageTemplateOptions.Value.Path}{message.MessageId}",
                                         cancellationToken
                                     ))?.FirstOrDefault();

        if (searchResult?.Id is null)
            return await CreatePageAsync(message, GenerateContent(message), cancellationToken);

        GetPage.Page? page = await _apiWrapper.GetPageAsync(searchResult.Id.Value, cancellationToken);

        if (page is not null)
            return await UpdatePageAsync(page, GenerateContent(message, page.Content), cancellationToken);

        return await CreatePageAsync(message, GenerateContent(message), cancellationToken);
    }

    private async Task<bool> CreatePageAsync(ReceivedMessage message,
                                             string content,
                                             CancellationToken cancellationToken
    )
    {
        Stopwatch timer = Stopwatch.StartNew();

        bool result = await _apiWrapper.CreatePageAsync(
                                           new()
                                           {
                                               Content = content,
                                               Description = _pageTemplateOptions.Value.Description,
                                               Editor = _pageTemplateOptions.Value.Editor,
                                               IsPrivate = _pageTemplateOptions.Value.IsPrivate,
                                               IsPublished = _pageTemplateOptions.Value.IsPublished,
                                               Locale = _pageTemplateOptions.Value.Locale,
                                               Title =
                                                   $"{_pageTemplateOptions.Value.Title} {message.MessageId}",
                                               Path = $"{_pageTemplateOptions.Value.Path}{message.MessageId}",
                                               Tags = _pageTemplateOptions.Value.Tags,
                                           }, cancellationToken
                                       )
                                       .ConfigureAwait(false);

        if (!result)
            _logger.LogError("Failed to create with message id: {MessageMessageId}", message.MessageId);

        timer.Stop();

        _logger.LogInformation(
            "Elapsed time to create page: {TimerElapsedMilliseconds} ms", timer.ElapsedMilliseconds
        );

        return result;
    }

    private string GenerateContent(ReceivedMessage message, string? baseContent = null)
    {
        StringBuilder sb = new(baseContent);

        if (!string.IsNullOrWhiteSpace(message.Message))
            sb.AppendLine(message.Message);

        if (!string.IsNullOrWhiteSpace(message.PhotoInBase64))
            sb.AppendLine($"![image]({message.PhotoInBase64})");

        return sb.ToString();
    }

    private async Task<bool> UpdatePageAsync(GetPage.Page page,
                                             string content,
                                             CancellationToken cancellationToken
    )
    {
        Stopwatch timer = Stopwatch.StartNew();

        bool result = await _apiWrapper.UpdatePageAsync(
                                           new()
                                           {
                                               Content = content,
                                               Description = page.Description,
                                               Editor = page.Editor,
                                               Id = page.Id,
                                               IsPublished = page.IsPublished,
                                               IsPrivate = page.IsPrivate,
                                               Locale = page.Locale,
                                               Path = page.Path,
                                               ScriptJs = page.ScriptJs,
                                               ScriptCss = page.ScriptCss,
                                               Tags = page.Tags,
                                               Title = page.Title,
                                           }, cancellationToken
                                       )
                                       .ConfigureAwait(false);

        if (!result)
            _logger.LogError("Failed to update page with path: {PagePath}", page.Path);

        timer.Stop();

        _logger.LogInformation(
            "Elapsed time to create page: {TimerElapsedMilliseconds} ms", timer.ElapsedMilliseconds
        );

        return result;
    }
}