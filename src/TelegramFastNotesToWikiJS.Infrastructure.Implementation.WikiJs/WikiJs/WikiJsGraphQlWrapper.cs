using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Options;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.Configurations;
using TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs.Models;

namespace TelegramFastNotesToWikiJS.Infrastructure.WikiJs.WikiJs;

public class WikiJsGraphQlWrapper
{
    private readonly GraphQLHttpClient _client;

    public WikiJsGraphQlWrapper(IOptions<WikiJsConfiguration> wikiJsOptions)
    {
        WikiJsConfiguration configuration = wikiJsOptions.Value;
        _client = new(configuration.BaseUri, new NewtonsoftJsonSerializer());
        _client.HttpClient.DefaultRequestHeaders.Authorization = new("Bearer", configuration.Token);
    }

    internal async Task<bool> CreatePageAsync(CreatePage.CreatePageRequest requestData,
                                              CancellationToken cancellationToken
    )
    {
        var variables = new
        {
            content = requestData.Content,
            description = requestData.Description,
            editor = requestData.Editor,
            isPublished = requestData.IsPublished,
            isPrivate = requestData.IsPrivate,
            locale = requestData.Locale,
            path = requestData.Path,
            tags = requestData.Tags?.ToArray(),
            title = requestData.Title,
        };

        GraphQLHttpRequest request = new()
        {
            Query = @"
            mutation CreatePage($content: String!, $description: String!, $editor: String!, $isPublished: Boolean!, $isPrivate: Boolean!, $locale: String!, $path: String!, $tags: [String]!, $title: String!) {
              pages {
                create(
                  content: $content
                  description: $description
                  editor: $editor
                  isPublished: $isPublished
                  isPrivate: $isPrivate
                  locale: $locale
                  path: $path
                  tags: $tags
                  title: $title
                ) {
                  page {
                    id
                  }
                }
              }
            }",
            Variables = variables,
        };

        GraphQLResponse<object> response = await _client.SendMutationAsync<object>(request, cancellationToken)
                                                        .ConfigureAwait(false);

        return response.Extensions is null;
    }

    internal async Task<GetPage.Page?> GetPageAsync(long id, CancellationToken cancellationToken)
    {
        var variables = new { id, };

        GraphQLHttpRequest request = new()
        {
            Query = @"
            query GetPage($id: Int!) {
              pages {
                single(id: $id) {
                    id,
                    content,
                    path,
                    title,
                    description,
                    editor,
                    isPrivate,
                    isPublished,
                    publishStartDate,
                    publishEndDate,
                    content,
                    locale,
                    scriptCss,
                    scriptJs,
                    tags {
                        tag
                    }
                }
              }
            }",
            Variables = variables,
        };

        GraphQLResponse<GetPage.Root> response =
            await _client.SendQueryAsync<GetPage.Root>(request, cancellationToken).ConfigureAwait(false);

        return response?.Data?.Pages?.Single;
    }

    internal async Task<IEnumerable<Search.Page>?> SearchPagesAsync(
        string query,
        CancellationToken cancellationToken
    )
    {
        GraphQLHttpRequest request = new()
        {
            Query = @"
                    query($query: String!) {
                        pages {
                            search(query: $query) {
                                results {
                                    id
                                    path
                                }
                            }
                        }
                    }",
            Variables = new { query, },
        };

        GraphQLResponse<Search.Root> response =
            await _client.SendQueryAsync<Search.Root>(request, cancellationToken).ConfigureAwait(false);

        return response.Data?.Pages?.Search?.Results;
    }

    internal async Task<bool> UpdatePageAsync(GetPage.Page requestData, CancellationToken cancellationToken)
    {
        var variables = new
        {
            id = requestData.Id,
            content = requestData.Content,
            description = requestData.Description,
            editor = requestData.Editor,
            isPrivate = requestData.IsPrivate,
            isPublished = requestData.IsPublished,
            locale = requestData.Locale,
            path = requestData.Path,
            publishEndDate = DateTime.UtcNow,
            publishStartDate = DateTime.UtcNow,
            scriptCss = requestData.ScriptCss,
            scriptJs = requestData.ScriptJs,
            tags = requestData.Tags?.Select(x => x.Tag),
            title = requestData.Title,
        };

        GraphQLHttpRequest request = new()
        {
            Query = @"
        mutation UpdatePage($id: Int!, $content: String, $description: String, $editor: String, $isPrivate: Boolean, $isPublished: Boolean, $locale: String, $path: String, $publishEndDate: Date, $publishStartDate: Date, $scriptCss: String, $scriptJs: String, $tags: [String], $title: String) {
            pages {
                update(
                    id: $id,
                    content: $content,
                    description: $description,
                    editor: $editor,
                    isPrivate: $isPrivate,
                    isPublished: $isPublished,
                    locale: $locale,
                    path: $path,
                    publishEndDate: $publishEndDate,
                    publishStartDate: $publishStartDate,
                    scriptCss: $scriptCss,
                    scriptJs: $scriptJs,
                    tags: $tags,
                    title: $title
                ) {
                    responseResult {
                        message
                    }
                }
            }
        }",
            Variables = variables,
        };

        GraphQLResponse<object> response = await _client.SendMutationAsync<object>(request, cancellationToken)
                                                        .ConfigureAwait(false);

        return response.Extensions is null;
    }
}