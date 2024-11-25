using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxNetDemo
{
    public class KeyPressThrottler : IDisposable
    {
        private readonly Dictionary<char, DateTime> _lastKeyPressTimes = new();
        private readonly TimeSpan _throttleInterval;
        private readonly Action<char> _onThrottledKeyPress;
        private readonly Action<char, TimeSpan> _onKeyStuck;

        public KeyPressThrottler(
            TimeSpan throttleInterval, 
            Action<char> onThrottledKeyPress,
            Action<char, TimeSpan> onKeyStuck)
        {
            _throttleInterval = throttleInterval;
            _onThrottledKeyPress = onThrottledKeyPress;
            _onKeyStuck = onKeyStuck;
        }

        public void OnKeyPress(char key)
        {
            var now = DateTime.Now;
            if (!_lastKeyPressTimes.TryGetValue(key, out var lastPress))
            {
                _lastKeyPressTimes[key] = now;
                _onThrottledKeyPress(key);
            }
            else
            {
                var timeSinceLastPress = now - lastPress;
                if (timeSinceLastPress >= _throttleInterval)
                {
                    _lastKeyPressTimes[key] = now;
                    _onThrottledKeyPress(key);
                }
                else
                {
                    _onKeyStuck(key, _throttleInterval - timeSinceLastPress);
                }
            }
        }

        public void Dispose()
        {
            _lastKeyPressTimes.Clear();
        }
    }
}
