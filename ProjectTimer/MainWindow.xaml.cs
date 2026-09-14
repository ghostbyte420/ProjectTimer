using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace ProjectTimer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<TimeEntry> Entries { get; } = new();

        private readonly DispatcherTimer _timer;
        private DateTime _startTime;
        private bool _isRunning;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _timer = new DispatcherTimer(DispatcherPriority.Send)
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var elapsed = DateTime.Now - _startTime;
            ElapsedTimeTextBlock.Text = elapsed.ToString(@"hh\:mm\:ss");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _startTime = DateTime.Now;
            _isRunning = true;

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            StatusTextBlock.Text = "Running";
            ElapsedTimeTextBlock.Text = "00:00:00";

            _timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isRunning)
            {
                return;
            }

            var endTime = DateTime.Now;
            _timer.Stop();
            _isRunning = false;

            var dialog = new StopDetailsWindow
            {
                Owner = this
            };

            var result = dialog.ShowDialog();

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            StatusTextBlock.Text = "Stopped";

            if (result == true)
            {
                Entries.Add(new TimeEntry
                {
                    StartTime = _startTime,
                    EndTime = endTime,
                    Reason = dialog.Reason,
                    Description = dialog.Description,
                    IsComplete = dialog.IsComplete,
                    IsIncomplete = dialog.IsIncomplete
                });
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Entries.Count == 0)
            {
                MessageBox.Show("There are no logged entries to export yet.", "Nothing to Export",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON file (*.json)|*.json",
                FileName = $"ProjectTimer_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(Entries, options);

            try
            {
                File.WriteAllText(saveDialog.FileName, json);
                MessageBox.Show("Export complete.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export: {ex.Message}", "Export Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "JSON file (*.json)|*.json"
            };

            if (openDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(openDialog.FileName);
                var importedEntries = JsonSerializer.Deserialize<List<TimeEntry>>(json);

                if (importedEntries is null || importedEntries.Count == 0)
                {
                    MessageBox.Show("No entries were found in the selected file.", "Nothing to Import",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (Entries.Count > 0)
                {
                    var appendResult = MessageBox.Show(
                        "Append imported entries to the current log? Choose No to replace the current log instead.",
                        "Import Entries", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (appendResult == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    if (appendResult == MessageBoxResult.No)
                    {
                        Entries.Clear();
                    }
                }

                foreach (var entry in importedEntries)
                {
                    Entries.Add(entry);
                }

                MessageBox.Show($"Imported {importedEntries.Count} entr{(importedEntries.Count == 1 ? "y" : "ies")}.",
                    "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import: {ex.Message}", "Import Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
