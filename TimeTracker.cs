using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxNetDemo
{
    public class TimeTracker : IDisposable
    {
        private readonly ISubject<long> _timeSubject;
        private readonly IDisposable _timer;
        private readonly DateTime _startTime;

        public TimeTracker()
        {
            _startTime = DateTime.Now;
            _timeSubject = new Subject<long>();
            
            _timer = Observable
                .Interval(TimeSpan.FromMilliseconds(1))
                .Subscribe(_ => 
                {
                    var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
                    _timeSubject.OnNext((long)elapsed);
                });
        }

        public IObservable<long> TimeStream => _timeSubject.AsObservable();

        public void Dispose()
        {
            _timer?.Dispose();
            (_timeSubject as IDisposable)?.Dispose();
        }
    }
}
