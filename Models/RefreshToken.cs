using System.ComponentModel.DataAnnotations.Schema;

namespace Nethues_ChatboxWebApp.Models
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresUtc { get; set; }
        public bool IsRevoked { get; set; }
    }
}
