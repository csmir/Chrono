using System;
using System.Timers;

namespace Chrono
{
    public class DateTimeEventArgs
    {
        /// <summary>
        ///     The UTC time of this raised event.
        /// </summary>
        public readonly DateTime GlobalTime;

        /// <summary>
        ///     The system time of this raised event.
        /// </summary>
        public readonly DateTime SystemTime;

        /// <summary>
        ///     The timer over which this event was raised.
        /// </summary>
        public readonly Timer Timer;

        /// <summary>
        ///     The event args of the timer that was raised.
        /// </summary>
        public readonly ElapsedEventArgs TimerElapsedArgs;

        /// <summary>
        /// Constructs a new type of <see cref="DateTimeEventArgs"/> with provided entries.
        /// </summary>
        /// <param name="globalTime"></param>
        /// <param name="systemTime"></param>
        /// <param name="timer"></param>
        /// <param name="args"></param>
        public DateTimeEventArgs(Timer timer, ElapsedEventArgs args, DateTime? globalTime = null, DateTime? systemTime = null)
        {
            GlobalTime = globalTime ?? DateTime.UtcNow;
            SystemTime = systemTime ?? DateTime.Now;
            Timer = timer;
            TimerElapsedArgs = args;
        }
    }
}
