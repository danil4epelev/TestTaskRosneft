using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TestTask.Models;

namespace TestTask
{
  public class ApplicationContext : DbContext
  {
    public DbSet<DirectoryBrand> DirectoryBrands { get; set; }

    public ApplicationContext() 
    { 
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
      : base(options) 
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (optionsBuilder != null && !optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseNpgsql(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
        optionsBuilder.EnableSensitiveDataLogging();
      }
    }
  }
}
