namespace TelegramFastNotesToWikiJS.Domain.Configurations;

public class TelegramConfiguration
{
    public long OwnerId { get; private set; } = 0;
    public string Token { get; private set; } = string.Empty;
}