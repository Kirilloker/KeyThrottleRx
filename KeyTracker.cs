using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace RxNetDemo
{
    public class KeyEvent
    {
        public char Key { get; set; }
        public long Timestamp { get; set; }
    }

    public class KeyTracker : IDisposable
    {
        private readonly ISubject<KeyEvent> _keySubject;

        public KeyTracker()
        {
            _keySubject = new Subject<KeyEvent>();
        }

        public IObservable<KeyEvent> KeyStream => _keySubject.AsObservable();

        public void TrackKeyPress(char key, long timestamp)
        {
            _keySubject.OnNext(new KeyEvent 
            { 
                Key = key,
                Timestamp = timestamp
            });
        }

        public void Dispose()
        {
            (_keySubject as IDisposable)?.Dispose();
        }
    }
}
