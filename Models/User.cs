using System.ComponentModel.DataAnnotations.Schema;

namespace Nethues_ChatboxWebApp.Models
{
    [Table("Users")]
    public class User
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("FirstName")]
        public required  string FirstName { get; set; }
        [Column("LastName")]
        public required string LastName { get; set; }
        [Column("Username")]
        public required string Username { get; set; }
        [Column("Password")]
        public required string PasswordHash { get; set; }
        [Column("DateTimeUTC")]
        public DateTime CreatedUtc { get; set; }
        [Column("IsActive")]
        public bool IsActive { get; set; }
    }
}
