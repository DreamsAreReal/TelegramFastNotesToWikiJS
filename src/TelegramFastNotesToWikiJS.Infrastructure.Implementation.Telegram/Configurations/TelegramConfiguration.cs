namespace TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram.Configurations;

public class TelegramConfiguration
{
    public long OwnerId { get; init; }
    public string Token { get; init; } = string.Empty;
}