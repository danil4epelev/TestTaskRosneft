using System.ComponentModel.DataAnnotations.Schema;


namespace TestTask.Models
{
  [Table("design_objects")]
  public class DesignObject : BaseEntity
  {
    [Column("name")]
    public string? Name { get; set; }

    [Column("code")]
    public string? Code { get; set; }

    [Column("project_id"), ForeignKey("Project")]
    public int? ProjectId { get; set; }

    public virtual Project Project { get; set; }

    [Column("parent_object_id"), ForeignKey("ParentObject")]
    public int? ParentObjectId { get; set; }

    public virtual DesignObject ParentObject { get; set; }
  }
}

