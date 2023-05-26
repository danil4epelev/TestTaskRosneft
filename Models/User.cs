using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
  [Table("users")]
  public class User
  {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("login")]
    public string Login { get; set; }

    [Column("hashpassword")]
    public string HashPassword { get; set; }
  }
}
