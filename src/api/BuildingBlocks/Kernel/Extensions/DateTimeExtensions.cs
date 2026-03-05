using System.Globalization;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    /// <summary>
    /// Extension methods for DateTime manipulation and formatting
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Formats a DateTime as dd/MM/yyyy
        /// </summary>
        /// <param name="dateTime">The date to format</param>
        /// <returns>Formatted date string</returns>
        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd'/'MM'/'yyyy");
        }

        /// <summary>
        /// Formats a DateTime as dd/MM/yyyy HH:mm:ss
        /// </summary>
        /// <param name="dateTime">The date and time to format</param>
        /// <returns>Formatted date and time string</returns>
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd'/'MM'/'yyyy HH:mm:ss");
        }

        /// <summary>
        /// Formats a DateTime as HH:mm
        /// </summary>
        /// <param name="dateTime">The time to format</param>
        /// <returns>Formatted time string</returns>
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }

        /// <summary>
        /// Formats a time range as "HH:mm - HH:mm"
        /// </summary>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        /// <returns>Formatted time range string</returns>
        public static string ToTimeRangeString(this DateTime startTime, DateTime endTime)
        {
            return $"{startTime:HH:mm} - {endTime:HH:mm}";
        }

        /// <summary>
        /// Combines the date from one DateTime with the time from another
        /// </summary>
        /// <param name="date">The DateTime to take the date from</param>
        /// <param name="time">The DateTime to take the time from</param>
        /// <returns>New DateTime with combined date and time</returns>
        public static DateTime CombineDateAndTime(this DateTime date, DateTime time)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute,
                time.Second,
                time.Kind
            );
        }

        /// <summary>
        /// Gets the localized day of week name
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <param name="culture">Culture to use (defaults to Spanish)</param>
        /// <returns>Localized day name</returns>
        public static string GetDayOfWeekName(this DateTime dateTime, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? new CultureInfo("es-ES");
            return cultureInfo.DateTimeFormat.GetDayName(dateTime.DayOfWeek);
        }

        /// <summary>
        /// Gets the localized day of week name
        /// </summary>
        /// <param name="dayOfWeek">The day of week</param>
        /// <param name="culture">Culture to use (defaults to Spanish)</param>
        /// <returns>Localized day name</returns>
        public static string GetDayName(this DayOfWeek dayOfWeek, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? new CultureInfo("es-ES");
            return cultureInfo.DateTimeFormat.GetDayName(dayOfWeek);
        }

        /// <summary>
        /// Formats a DateTime with day of week and date (e.g., "JUEVES 31/07/2021")
        /// </summary>
        /// <param name="dateTime">The date to format</param>
        /// <param name="culture">Culture to use (defaults to Spanish)</param>
        /// <returns>Formatted string with day name and date</returns>
        public static string ToFullDateString(this DateTime dateTime, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? new CultureInfo("es-ES");
            var dayName = cultureInfo.DateTimeFormat.GetDayName(dateTime.DayOfWeek).ToUpper();
            var dateString = dateTime.ToShortDateString();
            return $"{dayName} {dateString}";
        }

        /// <summary>
        /// Gets the time elapsed in a human-readable format (e.g., "2 days ago", "just now")
        /// </summary>
        /// <param name="dateTime">The past date</param>
        /// <param name="currentTime">The current time (defaults to UTC now)</param>
        /// <param name="culture">Culture for localization</param>
        /// <returns>Human-readable time elapsed</returns>
        public static string ToRelativeTimeString(this DateTime dateTime, DateTime? currentTime = null, CultureInfo? culture = null)
        {
            var now = currentTime ?? DateTime.UtcNow;
            var timeSpan = now - dateTime;

            // Note: This uses Spanish by default. For production, consider using resource files for i18n
            if (timeSpan.TotalDays >= 365)
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return years == 1 ? "hace 1 año" : $"hace {years} años";
            }

            if (timeSpan.TotalDays >= 30)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return months == 1 ? "hace 1 mes" : $"hace {months} meses";
            }

            if (timeSpan.TotalDays >= 2)
            {
                return $"hace {(int)timeSpan.TotalDays} días";
            }

            if (timeSpan.TotalDays >= 1)
            {
                return "ayer";
            }

            if (timeSpan.TotalHours >= 2)
            {
                return $"hace {(int)timeSpan.TotalHours} horas";
            }

            if (timeSpan.TotalHours >= 1)
            {
                return "hace más o menos una hora";
            }

            if (timeSpan.TotalMinutes >= 5)
            {
                return $"hace {(int)timeSpan.TotalMinutes} minutos";
            }

            return "ahora mismo";
        }

        /// <summary>
        /// Checks if a DateTime is today
        /// </summary>
        /// <param name="dateTime">The date to check</param>
        /// <returns>True if the date is today</returns>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// Checks if a DateTime is yesterday
        /// </summary>
        /// <param name="dateTime">The date to check</param>
        /// <returns>True if the date is yesterday</returns>
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// Checks if a DateTime is tomorrow
        /// </summary>
        /// <param name="dateTime">The date to check</param>
        /// <returns>True if the date is tomorrow</returns>
        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// Checks if a DateTime is in the past
        /// </summary>
        /// <param name="dateTime">The date to check</param>
        /// <param name="compareWith">Date to compare with (defaults to Now)</param>
        /// <returns>True if the date is in the past</returns>
        public static bool IsPast(this DateTime dateTime, DateTime? compareWith = null)
        {
            return dateTime < (compareWith ?? DateTime.Now);
        }

        /// <summary>
        /// Checks if a DateTime is in the future
        /// </summary>
        /// <param name="dateTime">The date to check</param>
        /// <param name="compareWith">Date to compare with (defaults to Now)</param>
        /// <returns>True if the date is in the future</returns>
        public static bool IsFuture(this DateTime dateTime, DateTime? compareWith = null)
        {
            return dateTime > (compareWith ?? DateTime.Now);
        }

        /// <summary>
        /// Gets the start of the day (00:00:00)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at start of day</returns>
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the end of the day (23:59:59.999)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at end of day</returns>
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Gets the start of the week (Monday 00:00:00)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at start of week</returns>
        public static DateTime StartOfWeek(this DateTime dateTime)
        {
            int diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dateTime.AddDays(-diff).Date;
        }

        /// <summary>
        /// Gets the end of the week (Sunday 23:59:59.999)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at end of week</returns>
        public static DateTime EndOfWeek(this DateTime dateTime)
        {
            return dateTime.StartOfWeek().AddDays(7).AddTicks(-1);
        }

        /// <summary>
        /// Gets the start of the month (day 1 at 00:00:00)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at start of month</returns>
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Gets the end of the month (last day at 23:59:59.999)
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <returns>DateTime at end of month</returns>
        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        /// Gets the age in years from a birthdate
        /// </summary>
        /// <param name="birthDate">The birth date</param>
        /// <param name="referenceDate">Reference date (defaults to today)</param>
        /// <returns>Age in years</returns>
        public static int GetAge(this DateTime birthDate, DateTime? referenceDate = null)
        {
            var reference = referenceDate ?? DateTime.Today;
            var age = reference.Year - birthDate.Year;

            if (reference.Month < birthDate.Month ||
                (reference.Month == birthDate.Month && reference.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }
    }
}
