using Microsoft.EntityFrameworkCore;
using TestTask.Models;

namespace TestTask
{
  public class ApplicationContext : DbContext
  {
    public DbSet<DirectoryBrand> DirectoryBrands => Set<DirectoryBrand>();
    public DbSet<DesignObject> DesignObjects => Set<DesignObject>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PackageDocumentation> PackageDocumentations => Set<PackageDocumentation>();

    public ApplicationContext()
    {

    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLazyLoadingProxies();
      if (optionsBuilder != null && !optionsBuilder.IsConfigured)
      {
        string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
          throw new Exception("Строка подключения не задана.");

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.EnableSensitiveDataLogging();
      }
    }
  }
}
