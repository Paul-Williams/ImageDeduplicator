# nullable enable

using System;

// See: https://stackoverflow.com/questions/1004698/how-to-truncate-milliseconds-off-of-a-net-datetime


namespace Data
{
  public static class DateTimeExtensions
  {

    public static TimeSpan Difference(this DateTime date1, DateTime date2) => new(Math.Abs(date1.Ticks - date2.Ticks));

    //public static bool IsGreaterThan(this TimeSpan timeSpan, TimeSpan interval) => timeSpan > interval;


    //// Example usage:
    ////  dateTime = dateTime.Truncate(TimeSpan.FromMilliseconds(1)); // Truncate to whole ms
    ////  dateTime = dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second
    ////  dateTime = dateTime.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute
    //public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
    //{
    //  //return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
    //  if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
    //  if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
    //  return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
    //}

    
    //public static DateTime WithoutMilliseconds(this DateTime dateTime) => dateTime.Truncate(TimeSpan.FromSeconds(1));

    //public static bool EqualsIgnoringMilliseconds(this DateTime date1, DateTime date2) =>
    //  date1.AddTicks(-(date1.Ticks % TimeSpan.TicksPerSecond)) == date2.AddTicks(-(date2.Ticks % TimeSpan.TicksPerSecond));

    //// The above code produces incorrect results in some cases. e.g:
    ////     Ticks: 636452700760000000     --          Ticks: 636452700759994487
    ////     TimeOfDay: {01:41:16}         --          TimeOfDay: {01:41:15.9994487}
    ////------------


    //// 2nd go :)
    //// See: https://stackoverflow.com/questions/766626/is-there-a-better-way-in-c-sharp-to-round-a-datetime-to-the-nearest-5-seconds

    // This didn't seem to work properly either. 

    //private static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType)
    //{
    //  return new TimeSpan(
    //      Convert.ToInt64(Math.Round(
    //          time.Ticks / (decimal)roundingInterval.Ticks,
    //          roundingType
    //      )) * roundingInterval.Ticks
    //  );
    //}

    //private static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval) => 
    //  Round(time, roundingInterval, MidpointRounding.ToEven);

    //public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval) => 
    //  new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);

    
    //static TimeSpan OneSecond = TimeSpan.FromSeconds(1);

    //public static DateTime ToNearestSecond(this DateTime datetime) =>
    //  new DateTime((datetime - DateTime.MinValue).Round(OneSecond).Ticks);

    //public static DateTime ToNearestMinute(this DateTime datetime) =>
    //  new DateTime((datetime - DateTime.MinValue).Round(OneMinute).Ticks);

  }
}
