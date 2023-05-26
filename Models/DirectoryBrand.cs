using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
  [Table("directory_brands")]
  public class DirectoryBrand : BaseEntity
  {
    [Column("code")]
    public string? Code { get; set; }

    [Column("name")]
    public string? Name { get; set; }
  }
}