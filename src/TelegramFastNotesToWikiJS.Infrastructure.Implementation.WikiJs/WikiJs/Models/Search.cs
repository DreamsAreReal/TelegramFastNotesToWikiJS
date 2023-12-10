namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs.Models;

internal static class Search
{
    public class Page
    {
        public long? Id { get; set; }
        public string? Path { get; set; }
    }

    public class Response
    {
        public IEnumerable<Page>? Results { get; set; }
    }

    public class Root
    {
        public SearchResponse? Pages { get; set; }
    }

    public class SearchResponse
    {
        public Response? Search { get; set; }
    }
}