using System;
using System.Reactive.Linq;

namespace RxNetDemo
{
    public class KeyPressFilter
    {
        private readonly IObservable<KeyEvent> _filteredKeyStream;

        public KeyPressFilter(IObservable<KeyEvent> keyStream)
        {
            _filteredKeyStream = keyStream
                .GroupBy(k => k.Key) // Группируем события по клавише
                .SelectMany(group => group
                    .Throttle(TimeSpan.FromMilliseconds(500)) 
                );
        }

        public IObservable<KeyEvent> FilteredStream => _filteredKeyStream;
    }
}
