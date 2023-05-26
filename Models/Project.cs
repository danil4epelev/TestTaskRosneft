using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
  [Table("projects")]
  public class Project : BaseEntity
  {
    [Column("name")]
    public string? Name { get; set; }

    [Column("code")]
    public string? Code { get; set; }
    
  }
}
