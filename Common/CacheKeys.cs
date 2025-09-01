namespace RESTful.Common;

public static class CacheKeys
{
    public static string UserById(int id) => $"user: {id}";
    public const string UsersAll = "users:all";
}