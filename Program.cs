using TestTask.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using TestTask.Logger;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TestTask
{
  public class Program
  {
    public static void Main(string[] args)
    {
      #region Первоначальные настройки приложения
      ApplicationContext db = new ApplicationContext();

      var builder = WebApplication.CreateBuilder();    
      
      builder.Services.AddControllers().AddJsonOptions(x =>
                      x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

      // Настройка авторизации
      builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options => options.LoginPath = "/login");
      builder.Services.AddAuthorization();

      // Настройка хранения логов
      string pathLog = System.Configuration.ConfigurationManager.AppSettings["LogsPath"];
      if (string.IsNullOrEmpty(pathLog))
        throw new Exception("Путь логирования не задан.");

      builder.Logging.AddFile(Path.Combine(
        pathLog,
        $"log-{DateTime.Now.ToString("d")}.log"));

      var app = builder.Build();

      // Указания приложению использовать авторизацию и аунтификацию
      app.UseAuthentication();
      app.UseAuthorization();

      // Указания приложению давать возможность использовать файлы (нужно для использования html страниц)
      app.UseDefaultFiles();
      app.UseStaticFiles();
      #endregion

      #region Основные страницы
      app.MapGet("/login", async (HttpContext context) =>
      {

        if (context.User.Identity.IsAuthenticated)
        {
          context.Response.Redirect("/home");
        }
        else
        {
          context.Response.ContentType = "text/html; charset=utf-8";
          await context.Response.SendFileAsync("wwwroot/login.html");
        }
      });

      app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
      {
        // получаем из формы email и пароль
        var form = context.Request.Form;
        // если email и/или пароль не установлены, посылаем статусный код ошибки 400
        if (!form.ContainsKey("login") || !form.ContainsKey("password"))
          return Results.BadRequest("Email и/или пароль не установлены");

        string email = form["login"];
        string password = form["password"];

        // находим пользователя 
        User? user = null;

        using (ApplicationContext db = new ApplicationContext())
        {
          var md5 = MD5.Create();
          var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
          var hashPassword = Convert.ToBase64String(hash);

          user = db.Users.Where(t => t.Login == email && hashPassword == t.HashPassword).FirstOrDefault();
        }

        // если пользователь не найден, отправляем статусный код 401
        if (user == null) return Results.Unauthorized();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };

        // создаем объект ClaimsIdentity
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

        // установка аутентификационных куки
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        return Results.Redirect(returnUrl ?? "/home");
      });

      app.MapGet("/logout", async (HttpContext context) =>
      {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Redirect("/login");
      });

      app.MapGet("/home", [Authorize] async (HttpContext context) =>
      {
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.SendFileAsync("wwwroot/index.html");
      });
      #endregion

      #region API

      #region GET запросы
      app.MapGet("/Api/{entity}", [Authorize] async (string entity) =>
      {
        Type entityObjectType = APIManager.GetEntityType(entity);
        if (entityObjectType == null)       
          return Results.NotFound();

        JsonSerializerOptions options = new()
        {
          ReferenceHandler = ReferenceHandler.Preserve
        };

        object entities = await APIManager.GetEntitiesAsync(app, entityObjectType, $"API, get all {entity}s", db);
        return Results.Json(entities, options);
      });

      app.MapGet("/Api/{entity}/{id}", async (string entity, int id) =>
      {
        app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: API, get {entity} by id {id}");

        try
        {
          Type entityType = APIManager.GetEntityType(entity);

          if (entityType == null)
            return Results.NotFound(new { message = $"{entity} не найден" });

          var entityObject = APIManager.GetEntityObjectById(entityType, db, id);
          if (entityObject == null)
            return Results.NotFound(new { message = $"{entity} не найден" });

          return Results.Json(entityObject);
        }
        catch (Exception ex)
        {
          app.Logger.LogError($"[{DateTime.Now.ToShortTimeString()}]: {ex.Message}");
          return Results.BadRequest(ex);
        }
      });
      #endregion

      #region POST запросы
      app.MapPost("/Api/{entity}", [Authorize] async (string entity, HttpContext context) =>
      {
        app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: API, create {entity}.");

        try
        {
          // Чтение JSON-тела запроса
          string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

          // Десериализация JSON в объект нужного типа
          Type entityType = APIManager.GetEntityType(entity);
          var entityObject = APIManager.ConvertToEntityObject(entityType, requestBody);

          if (entityObject == null)         
            return Results.BadRequest(new { message = $"Некорректный запрос для {entity}" });
          
          // Добавление объекта в базу данных
          db.Add(entityObject);
          await db.SaveChangesAsync();

          // Возвращение результата с кодом 201 (Created)
          string entityId = (entityObject as BaseEntity)?.Id.ToString();
          return Results.Created($"/api/{entity}/{entityId}", entityObject);
        }
        catch (Exception ex)
        {
          app.Logger.LogError($"[{DateTime.Now.ToShortTimeString()}]: {ex.Message}");
          return Results.BadRequest(ex);
        }
      });

      #endregion

      #region UPDATE запросы

      app.MapPut("/Api/{entity}/{id}", [Authorize] async (string entity, int id, HttpContext context) =>
      {
        app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: API, update {entity} with ID {id}.");

        try
        {
          // Чтение JSON-тела запроса
          string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

          // Десериализация JSON в объект нужного типа
          Type entityType = APIManager.GetEntityType(entity);
          var entityObject = APIManager.ConvertToEntityObject(entityType, requestBody);

          // Получение объекта из базы данных по указанному ID
          var existingEntity = await APIManager.GetEntityObjectById(entityType, db, id);

          if (existingEntity == null)          
            return Results.NotFound();
          

          // Обновление свойств объекта на основе полученных данных
          APIManager.UpdateEntityProperties(existingEntity, entityObject);

          await db.SaveChangesAsync();

          return Results.Ok();
        }
        catch (Exception ex)
        {
          app.Logger.LogError($"[{DateTime.Now.ToShortTimeString()}]: {ex.Message}");
          return Results.BadRequest(ex);
        }
      });
      #endregion

      #region DELETE запросы
      app.MapDelete("/Api/{entity}/{id}", [Authorize] async (string entity, int id) =>
      {
        app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: API, delete {entity} with ID {id}.");

        try
        {
          Type entityType = APIManager.GetEntityType(entity);
          // Получение объекта из базы данных по указанному ID
          var existingEntity = await APIManager.GetEntityObjectById(entityType, db, id);

          if (existingEntity == null)          
            return Results.NotFound();
          
          // Удаление объекта из базы данных
          db.Remove(existingEntity);
          await db.SaveChangesAsync();

          return Results.Ok();
        }
        catch (Exception ex)
        {
          app.Logger.LogError($"[{DateTime.Now.ToShortTimeString()}]: {ex.Message}");
          return Results.BadRequest(ex);
        }
      });
      #endregion

      #endregion

      app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: Starting the app");
      app.Run();

    }
  }

  

}