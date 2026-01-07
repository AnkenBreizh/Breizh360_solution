using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Breizh360.Data.Common;

internal static class EntityEntryExtensions
{
    public static bool HasProperty(this EntityEntry entry, string propertyName)
        => entry.Metadata.FindProperty(propertyName) is not null;

    public static void SetCurrentValue(this EntityEntry entry, string propertyName, object? value)
        => entry.Property(propertyName).CurrentValue = value;

    public static void TrySetCurrentValue(this EntityEntry entry, string propertyName, object? value)
    {
        if (entry.Metadata.FindProperty(propertyName) is null) return;
        entry.Property(propertyName).CurrentValue = value;
    }
}
