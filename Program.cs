using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.IO;

namespace RxNetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using var timeTracker = new TimeTracker();
            using var keyTracker = new KeyTracker();
            using var visualizer = new ConsoleGraphVisualizer();
            using var fileWriter = File.CreateText("keylog.txt");
            fileWriter.AutoFlush = true;

            // Создаем фильтр нажатий клавиш
            var keyFilter = new KeyPressFilter(keyTracker.KeyStream);

            // Словарь для подсчета частоты нажатий клавиш
            var keyFrequencies = new Dictionary<char, int>();

            // Словарь для отслеживания последнего времени нажатия каждой клавиши
            var lastKeyPress = new Dictionary<char, long>();

            var completed = false;

            // Подписываемся на отфильтрованный поток клавиш
            keyFilter.FilteredStream.Subscribe(
                keyEvent =>
                {
                    if (!keyFrequencies.ContainsKey(keyEvent.Key))
                        keyFrequencies[keyEvent.Key] = 0;
                    
                    keyFrequencies[keyEvent.Key]++;
                    visualizer.UpdateGraph(new Dictionary<char, int>(keyFrequencies));
                },
                ex => Console.WriteLine($"Ошибка: {ex.Message}"),
                () => completed = true
            );

            // Подписываемся на все нажатия для логирования
            keyTracker.KeyStream.Subscribe(
                keyEvent =>
                {
                    var message = $"{keyEvent.Timestamp}мс: Нажата клавиша {keyEvent.Key}";

                    // Проверяем залипание клавиши
                    if (lastKeyPress.ContainsKey(keyEvent.Key))
                    {
                        var timeSinceLastPress = keyEvent.Timestamp - lastKeyPress[keyEvent.Key];
                        if (timeSinceLastPress < 500)
                        {
                            var remainingTime = 500 - timeSinceLastPress;
                            message = $"{keyEvent.Timestamp}мс: Залипание клавиши {keyEvent.Key} (осталось {remainingTime}мс)";
                        }
                    }

                    lastKeyPress[keyEvent.Key] = keyEvent.Timestamp;
                    visualizer.AddLog(message);
                    fileWriter.WriteLine(message);
                },
                ex => Console.WriteLine($"Ошибка: {ex.Message}"),
                () => completed = true
            );

            Console.WriteLine("Нажмите Ctrl+Q для выхода");

            while (!completed)
            {
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    var timestamp = timeTracker.TimeStream.FirstAsync().Wait();
                    keyTracker.TrackKeyPress(keyInfo, timestamp);
                }
            }
        }
    }
}
