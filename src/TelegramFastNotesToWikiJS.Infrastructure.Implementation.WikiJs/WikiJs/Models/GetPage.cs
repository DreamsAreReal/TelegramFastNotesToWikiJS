namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs.Models;

internal static class GetPage
{
    public class Page
    {
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Editor { get; set; }
        public long? Id { get; set; }
        public bool? IsPrivate { get; set; }
        public bool? IsPublished { get; set; }
        public string? Locale { get; set; }
        public string? Path { get; set; }
        public string? ScriptCss { get; set; }
        public string? ScriptJs { get; set; }
        public IEnumerable<TagResponse>? Tags { get; set; }
        public string? Title { get; set; }
    }

    public class PageResponse
    {
        public Page? Single { get; set; }
    }

    public class Root
    {
        public PageResponse? Pages { get; set; }
    }

    public class TagResponse
    {
        public string? Tag { get; set; }
    }
}