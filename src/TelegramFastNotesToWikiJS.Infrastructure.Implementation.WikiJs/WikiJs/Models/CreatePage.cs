namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs.Models;

internal static class CreatePage
{
    internal class CreatePageRequest
    {
        internal string? Content { get; set; }
        internal string? Description { get; set; }
        internal string? Editor { get; set; }
        internal bool? IsPrivate { get; set; }
        internal bool? IsPublished { get; set; }
        internal string? Locale { get; set; }
        internal string? Path { get; set; }
        internal IEnumerable<string>? Tags { get; set; }
        internal string? Title { get; set; }
    }
}