namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs.Configurations;

public class WikiJsPageTemplateConfiguration
{
    public string Description { get; init; } = string.Empty;
    public string Editor { get; init; } = string.Empty;
    public bool IsPrivate { get; init; }
    public bool IsPublished { get; init; }
    public string Locale { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public IEnumerable<string>? Tags { get; init; }
    public string Title { get; init; } = string.Empty;
}