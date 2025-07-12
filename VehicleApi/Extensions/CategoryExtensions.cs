using VehicleApi.Models;

namespace VehicleApi.Extensions;

public static class CategoryExtensions
{
    public static IEnumerable<T> GetSpeedViolations<T>(this Category category, IEnumerable<Event> events, Func<DateTime, double, T> violationFactory)
    {
        var result = new List<T>();
        DateTime? violationStart = null;
        foreach (var ev in events)
        {
            if (ev.SpeedKm > category.SpeedLimitKm)
            {
                if (violationStart == null)
                    violationStart = ev.Timestamp;
                if ((ev.Timestamp - violationStart.Value).TotalSeconds >= category.SpeedLimitDurationSeconds)
                {
                    result.Add(violationFactory(violationStart.Value, (ev.Timestamp - violationStart.Value).TotalSeconds));
                    violationStart = null;
                }
            }
            else
            {
                violationStart = null;
            }
        }
        return result;
    }
}
