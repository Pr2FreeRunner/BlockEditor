using System;
using System.Threading;
using System.Windows.Threading;

namespace BlockEditor.Models
{
    public class GameEngine
    {
        private DispatcherTimer _timer;
        public const int FPS = 27;
        public const double MsPerFrame = 1000 / FPS;

        private bool _pause;
        private readonly object _pauseLock = new object();

        public bool Pause
        {
            get { return _pause; }
            set
            {
                lock (_pauseLock)
                {
                    _pause = value;
                }
            }
        }

        public event Action OnFrame;


        public GameEngine()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = TimeSpan.FromMilliseconds(MsPerFrame);
            _timer.Tick += OnTick;
        }

        
        public void PauseConfirmed()
        {
            Pause = false;
            Thread.Sleep((int)(MsPerFrame * 5)); // wait for engine to pause
        }

        public void Start()
        {
            _timer.Start();
        }

        public void RefreshGui()
        {
            if(!Pause)
                return;

            OnFrame?.Invoke();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Pause)
            {
                return;
            }

            OnFrame?.Invoke();
        }
    }


}
