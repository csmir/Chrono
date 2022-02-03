using System;
using System.Threading.Tasks;
using System.Timers;

namespace Chrono
{
    public class DateTimeClient
    {
        /// <summary>
        ///     Triggers on each second passed.
        /// </summary>
        public event Func<DateTimeEventArgs, Task> SecondPassed;

        /// <summary>
        ///     Triggers on each minute passed.
        /// </summary>
        public event Func<DateTimeEventArgs, Task> MinutePassed;

        /// <summary>
        ///     Triggers on each hour passed.
        /// </summary>
        public event Func<DateTimeEventArgs, Task> HourPassed;

        /// <summary>
        ///     Triggers on each day passed. Whether a day has passed is controlled by the <see cref="OffsetOptions"/>.
        /// </summary>
        public event Func<DateTimeEventArgs, Task> DayPassed;

        private readonly Timer _timer;
        private DateTime _time;
        private readonly OffsetOptions _type;

        /// <summary>
        ///     Constructs the <see cref="DateTimeClient"/> and starts the timer.
        /// </summary>
        /// <param name="type">The <see cref="OffsetOptions"/> to switch between system & global time.</param>
        public DateTimeClient(OffsetOptions type = OffsetOptions.Global)
        {
            _type = type;
            _time = _type.HasFlag(OffsetOptions.Global)
                ? DateTime.UtcNow
                : DateTime.Now;

            _timer = new(1000)
            {
                Enabled = true,
                AutoReset = true
            };
            _timer.Elapsed += Elapsed;
            _timer.Start();
        }

        /// <summary>
        ///     The approach to raise seperate events.
        /// </summary>
        internal enum RaiseType
        {
            /// <summary>
            ///     Raised on each second.
            /// </summary>
            Second,

            /// <summary>
            ///     Raised on each minute.
            /// </summary>
            Minute,

            /// <summary>
            ///     Raised on each hour.
            /// </summary>
            Hour,

            /// <summary>
            ///     Raised on each day.
            /// </summary>
            Day
        }

        private async void Elapsed(object sender, ElapsedEventArgs args)
        {
            var old = _time;
            var newtime = (_type == OffsetOptions.Global)
                ? DateTime.UtcNow
                : DateTime.Now;
            _time = newtime;

            if (old.Second != newtime.Second)
                await RaiseTimePassed(RaiseType.Second, args, DateTime.UtcNow, DateTime.Now);
            if (old.Minute != newtime.Second)
                await RaiseTimePassed(RaiseType.Minute, args, DateTime.UtcNow, DateTime.Now);
            if (old.Hour != newtime.Second)
                await RaiseTimePassed(RaiseType.Hour, args, DateTime.UtcNow, DateTime.Now);
            if (old.Day != newtime.Second)
                await RaiseTimePassed(RaiseType.Day, args, DateTime.UtcNow, DateTime.Now);
        }

        /// <summary>
        ///     Raises any <see cref="DateTimeClient"/> based on the provided <see cref="RaiseType"/>.
        /// </summary>
        /// <param name="type">The event type to raise.</param>
        /// <param name="args">The <see cref="ElapsedEventArgs"/> of this invoke.</param>
        /// <param name="utc">Current UTC time.</param>
        /// <param name="sys">Current system time.</param>
        internal virtual async Task RaiseTimePassed(RaiseType type, ElapsedEventArgs args, DateTime utc, DateTime sys)
        {
            switch (type)
            {
                case RaiseType.Second:
                    await (SecondPassed?.Invoke(new DateTimeEventArgs(_timer, args, utc, sys))
                        ?? Task.CompletedTask);
                    return;
                case RaiseType.Minute:
                    await (MinutePassed?.Invoke(new DateTimeEventArgs(_timer, args, utc, sys))
                        ?? Task.CompletedTask);
                    return;
                case RaiseType.Hour:
                    await (HourPassed?.Invoke(new DateTimeEventArgs(_timer, args, utc, sys))
                        ?? Task.CompletedTask);
                    return;
                case RaiseType.Day:
                    await (DayPassed?.Invoke(new DateTimeEventArgs(_timer, args, utc, sys))
                        ?? Task.CompletedTask);
                    return;
            }
        }
    }
}
