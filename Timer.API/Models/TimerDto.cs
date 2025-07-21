namespace Timer.API.Models
{
    public class TimerDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int RemainingSeconds { get; set; }
        public int DurationInSeconds { get; set; }
        public bool IsPaused { get; set; }
        public bool IsCompleted { get; set; }
    }
}
