using System;

namespace ProjectTimer
{
    public class TimeEntry
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public bool IsIncomplete { get; set; }

        public string StartTimeDisplay => StartTime.ToString("g");
        public string EndTimeDisplay => EndTime.ToString("g");
        public string DurationDisplay => Duration.ToString(@"hh\:mm\:ss");
    }
}
