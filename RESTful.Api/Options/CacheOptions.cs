namespace RESTful.Api.Options;

public class CacheOptions
{
    public int UserByIdAbsoluteSeconds { get; set; } = 120;
    public int UserByIdSlidingSeconds { get; set; } = 30;
}