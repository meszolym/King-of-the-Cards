using System;
using Splat;

namespace KC.Frontend.Client.Extensions;

public static class LocatorExtensions
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver provider)
    {
        var result = provider.GetService<T>();
        if (result == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T)} not found");
        }

        return result;
    }
}