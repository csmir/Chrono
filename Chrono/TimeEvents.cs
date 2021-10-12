using System;
using System.Threading.Tasks;
using System.Timers;

namespace Chrono
{
    public class TimeEvents
    {
        /// <summary>
        ///     Triggers on each second passed.
        /// </summary>
        public event Func<TimeEventArgs, Task> OnSecondPassed;

        /// <summary>
        ///     Triggers on each minute passed.
        /// </summary>
        public event Func<TimeEventArgs, Task> OnMinutePassed;

        /// <summary>
        ///     Triggers on each hour passed.
        /// </summary>
        public event Func<TimeEventArgs, Task> OnHourPassed;

        /// <summary>
        ///     Triggers on each day passed.
        /// </summary>
        public event Func<TimeEventArgs, Task> OnDayPassed;

        private readonly Timer _timer;
        private DateTime _time;
        private readonly InvokeType _type;

        /// <summary>
        ///     Constructs the <see cref="TimeEvents"/> and starts the timer.
        /// </summary>
        /// <param name="type">The <see cref="InvokeType"/> to switch between system & global time.</param>
        public TimeEvents(InvokeType type)
        {
            _type = type;
            _time = (type != InvokeType.Global)
                ? DateTime.Now
                : DateTime.UtcNow;
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
        protected enum RaiseType
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

        private void Elapsed(object sender, ElapsedEventArgs args)
        {
            var old = _time;
            var newtime = (_type == InvokeType.Global)
                ? DateTime.UtcNow
                : DateTime.Now;
            _time = newtime;
            _ = Task.Run( async () =>
           {
               if (old.Second != newtime.Second)
                   await RaiseTimePassed(RaiseType.Second, args, DateTime.UtcNow, DateTime.Now);
               if (old.Minute != newtime.Second)
                   await RaiseTimePassed(RaiseType.Minute, args, DateTime.UtcNow, DateTime.Now);
               if (old.Hour != newtime.Second)
                   await RaiseTimePassed(RaiseType.Hour, args, DateTime.UtcNow, DateTime.Now);
               if (old.Day != newtime.Second)
                   await RaiseTimePassed(RaiseType.Day, args, DateTime.UtcNow, DateTime.Now);
           });
        }

        /// <summary>
        ///     Raises any <see cref="TimeEvents"/> based on the provided <see cref="RaiseType"/>.
        /// </summary>
        /// <param name="type">The event type to raise.</param>
        /// <param name="args">The <see cref="ElapsedEventArgs"/> of this invoke.</param>
        /// <param name="utc">Current UTC time.</param>
        /// <param name="sys">Current system time.</param>
        protected virtual async Task RaiseTimePassed(RaiseType type, ElapsedEventArgs args, DateTime utc, DateTime sys)
        {
            switch (type)
            {
                case RaiseType.Second:
                    await (OnSecondPassed?.Invoke(new TimeEventArgs(_timer, args, utc, sys)) 
                        ?? Task.CompletedTask);
                    return;
                case RaiseType.Minute:
                    await (OnMinutePassed?.Invoke(new TimeEventArgs(_timer, args, utc, sys)) 
                        ?? Task.CompletedTask); 
                    return;
                case RaiseType.Hour:
                    await (OnHourPassed?.Invoke(new TimeEventArgs(_timer, args, utc, sys)) 
                        ?? Task.CompletedTask);
                    return;
                case RaiseType.Day:
                    await (OnDayPassed?.Invoke(new TimeEventArgs(_timer, args, utc, sys)) 
                        ?? Task.CompletedTask);
                    return;
            }
        }
    }
}
