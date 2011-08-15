#if SILVERLIGHT
namespace System.Diagnostics
{
    public interface IStopwatch
    {
        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in timer ticks.
        /// </summary>
        long ElapsedTicks { get; }

        /// <summary>
        /// Gets a value indicating whether the Stopwatch timer is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero.
        /// </summary>
        void Reset();

        /// <summary>
        /// Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
        /// </summary>
        void Restart();
    }

    /// <summary>
    /// Implements a Silverlight-based stopwatch to mimic System.Diagnostics.Stopwatch in the full .NET framework
    /// </summary>
    public class Stopwatch : IStopwatch
    {
        private DateTime _startTime;
        private bool _isRunning;
        private TimeSpan _elapsed;

        private Stopwatch()
        {
            _isRunning = false;
            _elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// A read-only TimeSpan representing the total elapsed time measured by the current instance.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (_isRunning)
                {
                    return _elapsed + (DateTime.Now - _startTime);
                }
                else
                {
                    return _elapsed;
                }
            }
        }

        /// <summary>
        /// A read-only long integer representing the total number of ticks measured by the current instance.
        /// </summary>
        public long ElapsedTicks
        {
            get { return Elapsed.Ticks; }
        }

        /// <summary>
        /// A read-only long integer representing the total number of milliseconds measured by the current instance.
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return (long)Elapsed.TotalMilliseconds; }
        }


        /// <summary>
        /// Gets a value indicating whether the Stopwatch timer is running.
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        /// <summary>
        /// Initializes a new Stopwatch instance, sets the elapsed time property to zero, and starts measuring elapsed time.
        /// </summary>
        /// <returns></returns>
        public static Stopwatch StartNew()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        /// <summary>
        /// Gets the current number of ticks in the timer mechanism.
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            if (!_isRunning)
            {
                _startTime = DateTime.Now;
                _isRunning = true;
            }
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop()
        {
            if(_isRunning)
            {
                _isRunning = false;
                _elapsed += DateTime.Now - _startTime;
            }
        }

        /// <summary>
        /// Stops time interval measurement and resets the elapsed time to zero.
        /// </summary>
        public void Reset()
        {
            Stop();
            _elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
        /// </summary>
        public void Restart()
        {
            Reset();
            Start();
        }
    }
}
#endif