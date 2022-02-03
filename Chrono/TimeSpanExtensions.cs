using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chrono
{
    public static class TimeSpanExtensions
    {
        private static readonly Lazy<IReadOnlyDictionary<string, Func<string, TimeSpan>>> _callback = new(ValueFactory);

        private static readonly Regex _regex = new(@"(\d*)\s*([a-zA-Z]*)\s*(?:and|,)?\s*", RegexOptions.Compiled);

        #region callback;
        private static IReadOnlyDictionary<string, Func<string, TimeSpan>> ValueFactory()
        {
            var callback = ImmutableDictionary.CreateBuilder<string, Func<string, TimeSpan>>();
            callback["second"] = Seconds;
            callback["seconds"] = Seconds;
            callback["sec"] = Seconds;
            callback["s"] = Seconds;
            callback["minute"] = Minutes;
            callback["minutes"] = Minutes;
            callback["min"] = Minutes;
            callback["m"] = Minutes;
            callback["hour"] = Hours;
            callback["hours"] = Hours;
            callback["h"] = Hours;
            callback["day"] = Days;
            callback["days"] = Days;
            callback["d"] = Days;
            callback["week"] = Weeks;
            callback["weeks"] = Weeks;
            callback["w"] = Weeks;
            callback["month"] = Months;
            callback["months"] = Months;
            return callback.ToImmutable();
        }

        private static TimeSpan Seconds(string match)
            => new(0, 0, int.Parse(match));

        private static TimeSpan Minutes(string match)
            => new(0, int.Parse(match), 0);

        private static TimeSpan Hours(string match)
            => new(int.Parse(match), 0, 0);

        private static TimeSpan Days(string match)
            => new(int.Parse(match), 0, 0, 0);

        private static TimeSpan Weeks(string match)
            => new((int.Parse(match) * 7), 0, 0, 0);

        private static TimeSpan Months(string match)
            => new((int.Parse(match) * 30), 0, 0, 0);
        #endregion

        /// <summary>
        ///     Gets a <see cref="TimeSpan"/> from a string.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to parse a <see cref="TimeSpan"/> from.</param>
        /// <param name="options">Parsing options for this call.</param>
        /// <param name="action">Initial data of the <see cref="TimeSpan"/>.</param>
        /// <returns>
        ///     A <see cref="TimeSpan"/> from any value held in the <paramref name="input"/>. 
        ///     
        ///     Will hold properties of the action if passed. If not and nothing was matched, <see cref="TimeSpan.Zero"/>
        /// </returns>
        public static TimeSpan GetTimeSpan(this string input, Action<TimeSpan> action, ParseOptions options = ParseOptions.None)
        {
            var span = TimeSpan.Zero;
            action(span);

            input = input.ToLower().Trim();

            if (!TimeSpan.TryParse(input, out TimeSpan parseResult))
            {
                MatchCollection matches = _regex.Matches(input);
                if (matches.Any())
                {
                    foreach (Match match in matches)
                        if (_callback.Value.TryGetValue(match.Groups[2].Value, out var callback))
                            parseResult += callback(match.Groups[1].Value);
                }
            }

            if (parseResult == TimeSpan.Zero && options.HasFlag(ParseOptions.ThrowIfNothingMatched))
                throw new ArgumentException("Timespan has no value; Passed string has no matches.", nameof(input));

            if (!options.HasFlag(ParseOptions.DecrementResult))
                span += parseResult;

            else if (span >= parseResult)
                span -= parseResult;

            else
                throw new ArgumentOutOfRangeException(nameof(input), $"Timespans cannot hold a negative value. actionSpan: {span} - parseResult: {parseResult} would have resulted in this.");

            return span;
        }

        /// <summary>
        ///     Gets a <see cref="TimeSpan"/> from a string.
        /// </summary>
        /// <param name="input">The <see cref="string"/> to parse a <see cref="TimeSpan"/> from.</param>
        /// <param name="options">Parsing options for this call.</param>
        /// <returns>
        ///     A <see cref="TimeSpan"/> from any value held in the <paramref name="input"/>. 
        ///     
        ///     Holds <see cref="TimeSpan.Zero"/> if nothing was matched.
        /// </returns>
        public static TimeSpan GetTimeSpan(this string input, ParseOptions options = ParseOptions.None)
            => GetTimeSpan(input, x => x = TimeSpan.Zero, options);
    }
}
