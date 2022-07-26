using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace BlockEditor.Models
{
    public class GameEngine
    {

        private System.Timers.Timer _timer;
        public const int FPS = 27;
        private bool _updating;

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
            _timer = new System.Timers.Timer();
            _timer.Interval = FPS;
            _timer.Elapsed += OnElapsed;
        }

        public void Start()
        {
            _timer.Enabled = true;
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Enabled = false;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_updating || Pause)
            {
                return;
            }

            _updating = true;

            Application.Current?.Dispatcher?.Invoke(DispatcherPriority.Render, new ThreadStart(delegate
            {
                OnFrame?.Invoke();
            }));

            _updating = false;
        }
    }


}
