using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{
  [Table("directory_brands")]
  public class DirectoryBrand
  {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    public string? Code { get; set; }

    [Column("name")]
    public string? Name { get; set; }
  }
}