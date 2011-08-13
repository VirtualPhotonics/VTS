using System;
using System.Collections.Generic;
using NLog;
using NLog.Targets;

namespace Vts.Common.Logging
{
    /// <summary>
    /// Writes log messages to an ArrayList in memory for programmatic retrieval.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Memory_target">Documentation on NLog Wiki</seealso>
    /// <example>
    /// <p>
    /// To set up the target in the <a href="config.html">configuration file</a>, 
    /// use the following syntax:
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/Memory/NLog.config" />
    /// <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    /// </p>
    /// <p>
    /// To set up the log target programmatically use code like this:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/Memory/Simple/Example.cs" />
    /// </example>
    [Target("Observable")]
    public sealed class ObservableTarget : TargetWithLayout, IObservable<string>
    {
        private Subject<string> _subject;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryTarget" /> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public ObservableTarget()
        {
            this.Logs = new List<string>();
            _subject = new Subject<string>();
        }

        /// <summary>
        /// Gets the list of logs gathered in the <see cref="MemoryTarget"/>.
        /// </summary>
        public IList<string> Logs { get; private set; }

        /// <summary>
        /// Renders the logging event message and adds it to the internal ArrayList of log messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            string msg = this.Layout.Render(logEvent);

            _subject.OnNext(msg);

            this.Logs.Add(msg);
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}