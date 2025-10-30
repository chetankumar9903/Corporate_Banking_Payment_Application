namespace Corporate_Banking_Payment_Application.Utilities
{
    public class DateTimeHelper
    {
        public static DateTime GetIndianTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            );
        }
    }
}
