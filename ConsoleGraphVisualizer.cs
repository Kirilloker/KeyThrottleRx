using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Spectre.Console;

namespace RxNetDemo
{
    public class ConsoleGraphVisualizer : IDisposable
    {
        private const int MaxKeysToShow = 20;
        private const int ScaleStepSize = 5;
        private readonly ConcurrentQueue<string> _logs = new();
        private const int MaxLogs = 10;
        private Dictionary<char, int> _currentFrequencies = new();

        public ConsoleGraphVisualizer()
        {
            Console.Clear();
        }

        public void AddLog(string logMessage)
        {
            try
            {
                if (_logs.Count >= MaxLogs)
                {
                    string? removed;
                    _logs.TryDequeue(out removed);
                }
                _logs.Enqueue(logMessage);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                _logs.Enqueue($"Ошибка добавления лога: {ex.Message}");
            }
        }

        public void UpdateGraph(Dictionary<char, int> keyFrequencies)
        {
            try
            {
                _currentFrequencies = new Dictionary<char, int>(keyFrequencies);
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                _logs.Enqueue($"Ошибка обновления графика: {ex.Message}");
            }
        }

        private void RefreshDisplay()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            DrawGraph(_currentFrequencies);
            DrawLogs();
        }

        private void DrawGraph(Dictionary<char, int> keyFrequencies)
        {
            try
            {
                if (keyFrequencies.Count > 0)
                {
                    var sortedKeys = keyFrequencies
                        .OrderByDescending(x => x.Value)
                        .Take(MaxKeysToShow)
                        .ToDictionary(x => x.Key, x => x.Value);

                    var chart = new BarChart()
                        .Width(60)
                        .Label("[green]Key Frequency[/]")
                        .CenterLabel();

                    foreach (var (key, value) in sortedKeys)
                        chart.AddItem(key.ToString(), value, Color.Green);

                    AnsiConsole.Write(chart);
                }
            }
            catch (Exception ex)
            {
                _logs.Enqueue($"Ошибка отрисовки графика: {ex.Message}");
            }
        }

        private void DrawLogs()
        {
            try
            {
                var logsArray = _logs.Reverse().Take(MaxLogs).Reverse().ToArray();
                if (logsArray.Length > 0)
                {
                    var panel = new Panel(string.Join("\n", logsArray))
                    {
                        Header = new PanelHeader("Logs"),
                        Border = BoxBorder.Rounded
                    }.BorderColor(Color.Yellow);

                    AnsiConsole.Write(panel);
                }
                
                AnsiConsole.MarkupLine("[yellow]Нажмите[/] [red]Ctrl+Q[/] [yellow]для выхода[/]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отрисовки логов: {ex.Message}");
            }
        }

        public void Dispose()
        {
            while (_logs.TryDequeue(out _)) { }
            _currentFrequencies.Clear();
        }
    }
}
