using System.Reflection;

public static class RelationshipHelper<TEntity>
{
    public static void ResolveRelationships(AppDbContext context, TEntity entity)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name.EndsWith("Id"))
            {
                var relatedEntityName = property.Name.Replace("Id", string.Empty);
                var relatedEntityType = context.Model.GetEntityTypes()
                    .FirstOrDefault(e => e.ClrType.Name == relatedEntityName)?.ClrType;

                if (relatedEntityType == null)
                    continue;

                var foreignKeyValue = property.GetValue(entity);
                if (foreignKeyValue == null)
                    continue;

                var relatedEntity = context.Find(relatedEntityType, foreignKeyValue);
                if (relatedEntity != null)
                {
                    var navigationProperty = properties.FirstOrDefault(p => p.Name == relatedEntityName);
                    navigationProperty?.SetValue(entity, relatedEntity);
                }
            }
        }
    }
}
