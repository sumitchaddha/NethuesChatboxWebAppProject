namespace Nethues_ChatboxWebApp.Models
{
    public class JwtToken
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; }
        public int RefreshExpiresDays { get; set; }
    }
}
