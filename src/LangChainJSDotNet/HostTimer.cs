using System;
using System.Threading;

namespace LangChainJSDotNet
{
    internal sealed class HostTimer
    {
        private Timer _timer;
        private Func<double> _callback = () => Timeout.Infinite;
        public void Initialize(dynamic callback) => _callback = () => (double)callback();
        public void Schedule(double delay)
        {
            if (delay < 0)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
            else
            {
                if (_timer == null) _timer = new Timer(_ => Schedule(_callback()));
                _timer.Change(TimeSpan.FromMilliseconds(delay), Timeout.InfiniteTimeSpan);
            }
        }
    }
}
