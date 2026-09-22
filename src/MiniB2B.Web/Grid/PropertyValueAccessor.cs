using System.Collections.Concurrent;
using System.Reflection;

namespace MiniB2B.Web.Grid;

public static class PropertyValueAccessor
{
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> Cache = new();

    public static object? GetValue(object row, string fieldName)
    {
        var property = Cache.GetOrAdd(
            (row.GetType(), fieldName),
            key => key.Item1.GetProperty(key.Item2, BindingFlags.Public | BindingFlags.Instance));

        return property?.GetValue(row);
    }
}