using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chrono
{
    /// <summary>
    ///     Gets time from strings.
    /// </summary>
    public class TimeParser
    {
        private delegate TimeSpan CallBackDelegate(string match);
        private readonly Dictionary<string, CallBackDelegate> _callBackResult = new();
        private readonly Regex _regex = new(@"(\d*)\s*([a-zA-Z]*)\s*(?:and|,)?\s*");

        /// <summary>
        ///     Constructs the <see cref="TimeParser"/> class by setting the values in the callback dictionary.
        /// </summary>
        public TimeParser()
        {
            _callBackResult["second"] = Seconds;
            _callBackResult["seconds"] = Seconds;
            _callBackResult["sec"] = Seconds;
            _callBackResult["s"] = Seconds;

            _callBackResult["minute"] = Minutes;
            _callBackResult["minutes"] = Minutes;
            _callBackResult["min"] = Minutes;
            _callBackResult["m"] = Minutes;

            _callBackResult["hour"] = Hours;
            _callBackResult["hours"] = Hours;
            _callBackResult["h"] = Hours;

            _callBackResult["day"] = Days;
            _callBackResult["days"] = Days;
            _callBackResult["d"] = Days;

            _callBackResult["week"] = Weeks;
            _callBackResult["weeks"] = Weeks;
            _callBackResult["w"] = Weeks;

            _callBackResult["month"] = Months;
            _callBackResult["months"] = Months;
        }

        // Returns a new timespan with the seconds added from the provided match.
        private TimeSpan Seconds(string match)
            => new(0, 0, int.Parse(match));

        // Returns a new timespan with the minutes added from the provided match.
        private TimeSpan Minutes(string match)
            => new(0, int.Parse(match), 0);

        // Returns a new timespan with the hours added from the provided match.
        private TimeSpan Hours(string match)
            => new(int.Parse(match), 0, 0);

        // Returns a new timespan with the days added from the provided match.
        private TimeSpan Days(string match)
            => new(int.Parse(match), 0, 0, 0);

        // Returns a new timespan with the weeks added from the provided match.
        private TimeSpan Weeks(string match)
            => new((int.Parse(match) * 7), 0, 0, 0);

        // Returns a new timespan with the months added from the provided match.
        private TimeSpan Months(string match)
            => new((int.Parse(match) * 30), 0, 0, 0);

        /// <summary>
        ///     Gets a valid <see cref="TimeSpan"/> from the provided <c>string</c>.
        /// </summary>
        /// <param name="input">The disposable <c>string</c> where the timespan originates from</param>
        /// <returns>A <see cref="TimeSpan"/> that holds all valid entries from the provided <c>string</c>.</returns>
        public TimeSpan GetSpanFromString(string input)
        {
            if (!TimeSpan.TryParse(input, out TimeSpan span))
            {
                _ = input.ToLower().Trim();
                MatchCollection matches = _regex.Matches(input);
                if (matches.Any())
                    foreach (Match match in matches)
                        if (_callBackResult.TryGetValue(match.Groups[2].Value, out CallBackDelegate callback))
                            span += callback(match.Groups[1].Value);
            }
            return span;
        }

        /// <summary>
        ///     Gets a <see cref="DateTime"/> from the provided string.
        /// </summary>
        /// <param name="input">The <c>string</c> to create a retrieved <see cref="TimeSpan"/> with. This <see cref="TimeSpan"/> will be appended to the <paramref name="output"/> result.</param>
        /// <param name="op">The operator with which the retrieved <see cref="TimeSpan"/> is appended to the <paramref name="output"/> result.</param>
        /// <param name="time">A <see cref="DateTime"/> to append the retrieved <see cref="TimeSpan"/> to. <c>null</c> or <see cref="DateTime.UtcNow"/> for global current time. <see cref="DateTime.Now"/> for current system time.</param>
        /// <param name="output">The <see cref="DateTime"/> result.</param>
        /// <returns><c>true</c> if the out value is more or less than the <paramref name="time"/>. <c>false</c> if the retrieved <see cref="TimeSpan"/> is zero.</returns>
        public bool GetFromString(string input, ParseOperator op, DateTime? time, out DateTime output)
        {
            var span = GetSpanFromString(input);
            output = (op == ParseOperator.Add)
                ? time ?? DateTime.UtcNow + span
                : time ?? DateTime.UtcNow - span;
            return span != TimeSpan.Zero;
        }
    }
}
