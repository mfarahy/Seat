using Exir.Framework.Common;
using Exir.Framework.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeatService
{
    [SingletonLogger]
    public class MemoryLogAdapter : LogAdapter
    {
        private ConcurrentDictionary<int, StringBuilder> _sb;
        private string _template;
        private bool _start;

        public MemoryLogAdapter()
        {
            _template = "{date}|{level}|{message}|{newline}{exception}";
            _sb = new ConcurrentDictionary<int, StringBuilder>();
        }
        public override void Debug(object message, Exception exception)
        {
            log(LogLevels.Debug, message, exception);
        }

        public void Start()
        {
            _start = true;
            if (!_sb.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                _sb.TryAdd(Thread.CurrentThread.ManagedThreadId, new StringBuilder());
        }

        public string Stop()
        {
            _start = false;

            if (_sb != null && _sb.ContainsKey(Thread.CurrentThread.ManagedThreadId))
            {
                var sb = _sb[Thread.CurrentThread.ManagedThreadId];
                var result = sb.ToString();
                sb.Clear();
                sb = null;
                _sb.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
                return result;
            }
            return String.Empty;
        }

        private void log(LogLevels level, object message, Exception exception)
        {
            if (!_start) return;
            StringBuilder sb = null;
            if (_sb != null && _sb.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                sb = _sb[Thread.CurrentThread.ManagedThreadId];
            if (sb != null)
            {
                Dictionary<string, object> rp = new Dictionary<string, object>();
                rp.Add("level", level.ToString());
                rp.Add("date", DateTime.Now.ToString("HH:mm:ss,fff"));
                rp.Add("logger", Name);
                rp.Add("message", message);
                rp.Add("exception", exception?.SerializeToString() ?? String.Empty);
                rp.Add("newline", Environment.NewLine);
                string finalMessage = StringFormatUtility.ReplaceParams(_template, rp, "{", "}");
                sb.Append(finalMessage);
            }
        }

        public override void Error(object message, Exception exception)
        {
            log(LogLevels.Error, message, exception);
        }

        public override void Fatal(object message, Exception exception)
        {
            log(LogLevels.Fatal, message, exception);
        }

        public override void Info(object message, Exception exception)
        {
            log(LogLevels.Information, message, exception);
        }

        public override void Trace(object message, Exception exception)
        {
            log(LogLevels.Trace, message, exception);
        }

        public override void Warn(object message, Exception exception)
        {
            log(LogLevels.Warning, message, exception);
        }
    }

    public class MemoryLogAppender : LogAppender<MemoryLogAppender>
    {
        public override MemoryLogAppender Convert()
        {
            return this;
        }
    }
}
