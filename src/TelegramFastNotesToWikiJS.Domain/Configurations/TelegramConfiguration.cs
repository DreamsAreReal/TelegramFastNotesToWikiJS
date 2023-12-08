namespace TelegramFastNotesToWikiJS.Domain.Configurations;

public class TelegramConfiguration
{
    public long OwnerId { get; init; }
    public string Token { get; init; } = string.Empty;
}