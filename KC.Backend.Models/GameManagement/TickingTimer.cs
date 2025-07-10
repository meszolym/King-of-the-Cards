using Timer = System.Timers.Timer;

namespace KC.Backend.Models.GameManagement;

public class TickingTimer
{
    private Timer timer;
    private int totalSeconds;

    public int ElapsedSeconds { get; private set; }

    public int RemainingSeconds => totalSeconds - ElapsedSeconds;

    public bool Enabled => timer.Enabled;
    public event EventHandler? Tick;
    public event EventHandler? Elapsed;
    

    public class TickEventArgs(int elapsedSeconds, int remainingSeconds) : EventArgs
    {
        public int ElapsedSeconds { get; } = elapsedSeconds;
        public int RemainingSeconds { get; } = remainingSeconds;
    }

    public void Zero()
    {
        Stop();
        ElapsedSeconds = 0;
    }
    
    public TickingTimer(TimeSpan timeSpan)
    {
        timer = new Timer(TimeSpan.FromSeconds(1));
        timer.AutoReset = true;

        totalSeconds = (int)timeSpan.TotalSeconds;

        timer.Elapsed += (sender, args) =>
        {
            ElapsedSeconds++;
            if (ElapsedSeconds >= totalSeconds)
            {
                timer.Stop();
                Elapsed?.Invoke(this, EventArgs.Empty);
            }
            Tick?.Invoke(this, new TickEventArgs(ElapsedSeconds, RemainingSeconds));
        };
    }

    public void Start()
    {
        ElapsedSeconds = 0;
        timer.Start();
    }

    public void Stop() => timer.Stop();
    
    public void Reset()
    {
        Stop();
        ElapsedSeconds = 0;
        Start();
    }
}

    

