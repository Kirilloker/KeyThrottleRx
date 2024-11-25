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
        private bool _isCompleted;

        public KeyTracker()
        {
            _keySubject = new Subject<KeyEvent>();
            _isCompleted = false;
        }

        public IObservable<KeyEvent> KeyStream => _keySubject.AsObservable();

        public void TrackKeyPress(ConsoleKeyInfo keyInfo, long timestamp)
        {
            if (_isCompleted) return;

            // Проверяем на Ctrl+Q
            if (keyInfo.Key == ConsoleKey.Q && (keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
            {
                Complete();
                return;
            }

            _keySubject.OnNext(new KeyEvent 
            { 
                Key = keyInfo.KeyChar,
                Timestamp = timestamp
            });
        }

        public void Complete()
        {
            if (!_isCompleted)
            {
                _isCompleted = true;
                _keySubject.OnCompleted();
            }
        }

        public void Dispose()
        {
            Complete();
            (_keySubject as IDisposable)?.Dispose();
        }
    }
}
