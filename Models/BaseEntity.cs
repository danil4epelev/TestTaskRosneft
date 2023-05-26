using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
  public class BaseEntity
  {
    [Key]
    [Column("id")]
    public int Id { get; set; }
  }
}
