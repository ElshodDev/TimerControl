using Microsoft.AspNetCore.Mvc;
using Timer.API.Models;

namespace Timer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimerController : ControllerBase
    {
        private static readonly object _lock = new();
        private readonly TimerDbContext _context;

        public TimerController(TimerDbContext context) => _context = context;

        private Timer.API.Models.Timer? GetActiveTimer()
        {
            return _context.Timers
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefault(t => !t.IsCompleted);
        }

        private void CompleteExpiredTimers()
        {
            var now = DateTime.UtcNow;
            var expiredTimers = _context.Timers
                .Where(t => !t.IsCompleted && !t.IsPaused && t.EndTime <= now)
                .ToList();

            foreach (var timer in expiredTimers)
            {
                timer.IsCompleted = true;
                timer.RemainingSeconds = 0;
            }

            if (expiredTimers.Any())
                _context.SaveChanges();
        }

        [HttpPost("start")]
        public IActionResult StartTimer(int seconds)
        {
            lock (_lock)
            {
                CompleteExpiredTimers();

                var activeTimer = GetActiveTimer();
                if (activeTimer != null && !activeTimer.IsCompleted && !activeTimer.IsPaused)
                {
                    return BadRequest("Avvalgi taymer hali tugamagan.");
                }

                DateTime now = DateTime.UtcNow;

                var newTimer = new Timer.API.Models.Timer
                {
                    StartTime = now,
                    DurationInSeconds = seconds,
                    EndTime = now.AddSeconds(seconds),
                    IsCompleted = false,
                    IsPaused = false,
                    RemainingSeconds = seconds
                };

                _context.Timers.Add(newTimer);
                _context.SaveChanges();

                return Ok(new { message = "Taymer boshlandi." });
            }
        }

        [HttpPost("pause")]
        public IActionResult PauseTimer()
        {
            lock (_lock)
            {
                CompleteExpiredTimers();

                var timer = GetActiveTimer();
                if (timer == null || timer.IsCompleted)
                    return BadRequest("Taymer mavjud emas yoki allaqachon tugagan.");

                if (timer.IsPaused)
                    return BadRequest("Taymer allaqachon to'xtatilgan.");

                TimeSpan remaining = timer.EndTime.Value - DateTime.UtcNow;
                timer.RemainingSeconds = (int)Math.Max(0, remaining.TotalSeconds);
                timer.IsPaused = true;
                timer.LastPausedAt = DateTime.UtcNow;

                _context.Timers.Update(timer);
                _context.SaveChanges();

                return Ok(new { message = "Taymer to'xtatildi." });
            }
        }

        [HttpPost("resume")]
        public IActionResult ResumeTimer()
        {
            lock (_lock)
            {
                CompleteExpiredTimers();

                var timer = GetActiveTimer();
                if (timer == null || timer.IsCompleted)
                    return BadRequest("Taymer mavjud emas yoki tugagan.");

                if (!timer.IsPaused)
                    return BadRequest("Taymer hozir to'xtatilmagan.");

                timer.EndTime = DateTime.UtcNow.AddSeconds(timer.RemainingSeconds ?? 0);
                timer.IsPaused = false;
                timer.LastPausedAt = null;

                _context.SaveChanges();

                return Ok(new { message = "Taymer davom ettirildi." });
            }
        }

        [HttpGet("status")]
        public IActionResult GetTimerStatus()
        {
            lock (_lock)
            {
                CompleteExpiredTimers();

                var timer = GetActiveTimer();
                if (timer == null)
                    return NotFound("Taymer topilmadi.");

                var remaining = timer.IsPaused
                    ? timer.RemainingSeconds ?? 0
                    : (int)Math.Max(0, (timer.EndTime - DateTime.UtcNow)?.TotalSeconds ?? 0);

                var dto = new TimerDto
                {
                    StartTime = timer.StartTime,
                    EndTime = timer.EndTime,
                    RemainingSeconds = remaining,
                    DurationInSeconds = timer.DurationInSeconds,
                    IsPaused = timer.IsPaused,
                    IsCompleted = timer.IsCompleted
                };

                return Ok(dto);
            }
        }

        [HttpGet("history")]
        public IActionResult GetTimerHistory()
        {
            lock (_lock)
            {
                CompleteExpiredTimers();

                var history = _context.Timers
                    .OrderByDescending(t => t.StartTime)
                    .Select(t => new TimerDto
                    {
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        RemainingSeconds = t.RemainingSeconds ?? 0,
                        DurationInSeconds = t.DurationInSeconds,
                        IsPaused = t.IsPaused,
                        IsCompleted = t.IsCompleted
                    })
                    .ToList();

                return Ok(history);
            }
        }
    }
}
