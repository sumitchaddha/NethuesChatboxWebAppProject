using System.ComponentModel.DataAnnotations.Schema;

namespace Nethues_ChatboxWebApp.Models
{   
    [Table("UserActionsHistory")]
    public class UserActionsHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Action { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
