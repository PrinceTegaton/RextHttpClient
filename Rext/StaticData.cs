namespace Rext;

internal class StaticMessages
{
    public static string DeserializationFailure = "Unable to deserialize object to specified type of '{0}'";
}

internal class StaticObjects
{
    public static JsonSerializerOptions JsonSerializerOptionsForRequestObjects => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static JsonSerializerOptions JsonSerializerOptionsForResponseObjects => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}