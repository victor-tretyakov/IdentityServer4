using System.Collections.Generic;
using System.Text.Json;

namespace IdentityServer4.EntityFramework.Mappers;

internal static class PropertiesConverter
{
    public static string Convert(Dictionary<string, string> sourceMember)
    {
        return JsonSerializer.Serialize(sourceMember);
    }

    public static Dictionary<string, string> Convert(string sourceMember)
    {
        if (string.IsNullOrWhiteSpace(sourceMember))
        {
            return new Dictionary<string, string>();
        }
        return JsonSerializer.Deserialize<Dictionary<string, string>>(sourceMember);
    }
}