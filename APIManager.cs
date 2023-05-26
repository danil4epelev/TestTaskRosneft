using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace TestTask
{
  public class APIManager
  {
    public static Type GetEntityType(string entityTypeName)
    {
      string fullTypeName = $"TestTask.Models.{entityTypeName}"; // Замените "Namespace" на реальное пространство имен
      Type entityType = Type.GetType(fullTypeName);

      return entityType;
    }

    /// <summary>
    /// Асинхронно получить лист сущностей.
    /// </summary>
    /// <param name="app">Экземпляр приложения.</param>
    /// <param name="entityObjectType">Тип получаемой сущности.</param>
    /// <param name="logMessage">Лог-сообщение.</param>
    /// <param name="db">Контекст базы данных.</param>
    /// <returns>Лист из сущностей.</returns>
    public static async Task<object> GetEntitiesAsync(WebApplication app, Type entityObjectType, string logMessage, ApplicationContext db)
    {
      app.Logger.LogInformation($"[{DateTime.Now.ToShortTimeString()}]: {logMessage}");

      // Получаем MethodInfo для метода Set<T>()
      var setMethod = typeof(DbContext).GetMethods()
        .Where(t => t.Name == "Set" && t.GetParameters().Length == 0 && t.GetGenericArguments().Length == 1)
        .FirstOrDefault()
        ?.MakeGenericMethod(entityObjectType);

      if (setMethod == null)
        throw new Exception("Метод Set не найден в репозитории ApplicationContext.");

      // Вызываем метод Set<T>() и сохраняем результат в переменную
      var set = setMethod.Invoke(db, null);

      // Получаем MethodInfo для метода ToListAsync()
      var toListMethod = typeof(EntityFrameworkQueryableExtensions)
          .GetMethod("ToListAsync")
          .MakeGenericMethod(entityObjectType);

      // Вызываем метод ToListAsync() и сохраняем результат в переменную
      var entitiesTask = (Task)toListMethod.Invoke(null, new[] { set, null });

      // Ожидаем выполнения задачи и получаем результат
      await entitiesTask;
      var entities = entitiesTask.GetType().GetProperty("Result").GetValue(entitiesTask);

      return entities;
    }

    /// <summary>
    /// Получить сущность по id
    /// </summary>
    /// <param name="entity">наименование сущности</param>
    /// <param name="db">Репозиторий сущностей.</param>
    /// <param name="id">ид сущности</param>
    public static async Task<object> GetEntityObjectById(Type entityType, ApplicationContext db, int id)
    {
      var methods = typeof(DbContext).GetMethods().Where(t => t.Name == "Set");
      // Получаем MethodInfo для метода Set<T>()
      var setMethod = typeof(DbContext).GetMethods()
        .Where(t => t.Name == "Set" && t.GetParameters().Length == 0 && t.GetGenericArguments().Length == 1)
        .FirstOrDefault()
        ?.MakeGenericMethod(entityType);
      
      // Вызываем метод Set<T>() для получения IQueryable<T>
      var queryable = setMethod.Invoke(db, null) as IQueryable<object>;

      if (queryable == null)
        throw new NotSupportedException($"Сущность '{entityType.Name}' не поддерживается.");

      var parameter = Expression.Parameter(entityType, "t");
      var idProperty = entityType.GetProperty("Id");
      var idEqual = Expression.Equal(
          Expression.Property(parameter, idProperty),
          Expression.Constant(id));
      var lambda = Expression.Lambda(idEqual, parameter);

      var whereMethod = typeof(Queryable)
          .GetMethods()
          .First(method => method.Name == "Where" && method.GetParameters().Length == 2)
          .MakeGenericMethod(entityType);

      var whereResult = whereMethod.Invoke(null, new object[] { queryable, lambda });

      var firstOrDefaultMethod = typeof(Queryable)
        .GetMethods()
        .First(method => method.Name == "FirstOrDefault" && method.GetParameters().Length == 1)
        .MakeGenericMethod(entityType);

      var entityObject = firstOrDefaultMethod.Invoke(null, new object[] { whereResult });

      return await Task.FromResult(entityObject);

    }

    /// <summary>
    /// Преобразовать json в нужный тип.
    /// </summary>
    /// <param name="entity">наименование типа.</param>
    /// <param name="requestBody">json который необходимо преобразовать</param>
    /// <returns>Объект нужно типа</returns>
    public static object ConvertToEntityObject(Type entityType, string requestBody)
    {      
      return JsonConvert.DeserializeObject(requestBody, entityType);
    }

    /// <summary>
    /// Обновить свойства сущности.
    /// </summary>
    /// <param name="existingEntity">Обновляемая сущность.</param>
    /// <param name="updatedEntity">Сущность на основе которой идёт обновление.</param>
    public static void UpdateEntityProperties(object existingEntity, object updatedEntity)
    {
      var existingProperties = existingEntity.GetType().GetProperties();
      var updatedProperties = updatedEntity.GetType().GetProperties();

      foreach (var updatedProperty in updatedProperties)
      {
        if (updatedProperty.Name == "Id")
          continue;

        var existingProperty = existingProperties.FirstOrDefault(p => p.Name == updatedProperty.Name);
        if (existingProperty != null && existingProperty.CanWrite)
        {
          var updatedValue = updatedProperty.GetValue(updatedEntity);
          existingProperty.SetValue(existingEntity, updatedValue);
        }
      }
    }
  }
}
