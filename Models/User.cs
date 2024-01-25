using System.ComponentModel.DataAnnotations.Schema;


namespace taskmanagementAPI.Models
{
    [Table("users")]
    public class User
    {
        [Column("userid")]
        public int UserId { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("passwordhash")]
        public byte[] PasswordHash { get; set; }
        [Column("passwordsalt")]
        public byte[] PasswordSalt { get; set; }
    }
}