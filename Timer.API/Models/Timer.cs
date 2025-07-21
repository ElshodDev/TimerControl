namespace Timer.API.Models
{
    public class Timer
    {
        public int Id { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int DurationInSeconds { get; set; } 

        public bool IsCompleted { get; set; }

        public bool IsPaused { get; set; }
        public DateTime? LastPausedAt { get; set; }

        public int? RemainingSeconds { get; set; }
    }
}
