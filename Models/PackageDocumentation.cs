using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTask.Models
{ 
  [Table("package_documentations")]
  public class PackageDocumentation: BaseEntity
  {
    [Column("design_object"), ForeignKey("DesignObject")]
    public int? DesignObjectId { get; set; }

    public virtual DesignObject DesignObject { get; set; }

    [Column("brand"), ForeignKey("Brand")]
    public int? BrandId { get; set; }

    [Column("brand")]
    public virtual DirectoryBrand Brand { get; set; }
  }
}
