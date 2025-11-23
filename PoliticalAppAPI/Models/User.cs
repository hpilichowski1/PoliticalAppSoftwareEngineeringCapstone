using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    [Required, Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required, Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required, Column("role")]
    public string Role { get; set; } = "citizen";

    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;
}