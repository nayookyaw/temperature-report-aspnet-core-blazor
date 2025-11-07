namespace BackendAspNetCore.Utils;

public class DatetimeUtil
{
    public static DateTimeOffset GetCurrentUtcDatetime()
    {
        return DateTime.UtcNow;
    }
}