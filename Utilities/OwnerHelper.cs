public static class OwnerHelper<TEntity>
{
    public static void SetAnonymousOwner(TEntity entity, string? anonymousToken)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (anonymousToken == null) throw new ArgumentNullException(nameof(anonymousToken));

        var ownerProperty = entity.GetType().GetProperty("OwnerId");
        if (ownerProperty == null)
            throw new InvalidOperationException("OwnerId property not found in the entity.");

        if (ownerProperty.GetValue(entity) == null)
            ownerProperty.SetValue(entity, anonymousToken);
    }

    public static async Task EnsureAuthorizedOwner(
        TEntity entity,
        string? currentUserId,
        AppDbContext context,
        Func<TEntity, object?> ownerResolver)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (currentUserId == null) throw new UnauthorizedAccessException("Unauthorized.");

        var owner = ownerResolver(entity);
        if (owner == null)
        {
            if (entity.GetType().GetProperty("Organizer")?.PropertyType == typeof(User))
            {
                var organizer = await context.Users.FindAsync(long.Parse(currentUserId));
                if (organizer == null)
                    throw new InvalidOperationException("Owner not found for the current user.");
                entity.GetType().GetProperty("Organizer")?.SetValue(entity, organizer);
            }
            else
            {
                entity.GetType().GetProperty("OwnerId")?.SetValue(entity, long.Parse(currentUserId));
            }
        }
    }
}
