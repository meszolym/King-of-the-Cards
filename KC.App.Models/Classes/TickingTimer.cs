using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace KC.App.Models.Classes
{
    public class TickingTimer
    {
        private Timer timer;
        private int totalSeconds;

        public int ElapsedSeconds { get; private set; }

        public int RemainingSeconds => totalSeconds - ElapsedSeconds;

        public bool Enabled => timer.Enabled;
        public event EventHandler Tick;
        public event EventHandler Elapsed;
        

        public class TickEventArgs(int elapsedSeconds, int remainingSeconds) : EventArgs
        {
            public int ElapsedSeconds { get; } = elapsedSeconds;
            public int RemainingSeconds { get; } = remainingSeconds;
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
    }

    
}
