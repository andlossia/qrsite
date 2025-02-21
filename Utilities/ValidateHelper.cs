using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations;

public static class ValidateHelper<TEntity> where TEntity : class
{
    public static bool ValidateQuery(AppDbContext context, string query)
    {
        try
        {
            context.Set<TEntity>().Where(query).Take(1).ToList();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid filter expression: {ex.Message}");
            return false;
        }
    }

    public static bool ValidateSort(AppDbContext context, string sort)
    {
        try
        {
            context.Set<TEntity>().OrderBy(sort).Take(1).ToList();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void ValidateRequiredFields(TEntity entity)
    {
        var requiredProperties = entity.GetType().GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute)));

        foreach (var property in requiredProperties)
        {
            var value = property.GetValue(entity);
            if (value == null)
                throw new ValidationException($"{property.Name} is required.");
        }
    }
}
