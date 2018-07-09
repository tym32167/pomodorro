using pomodorro.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace pomodorro
{
    public class MainModel : INotifyPropertyChanged
    {
        private DateTime EndTime = DateTime.UtcNow;
        private readonly Timer _timer;

        private TimeSpan _timeLeft;
        private Brush _foregroundColor;

        public MainModel()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;

            RestartCommand = new DelegateCommand<MainModel>(Restart);
            SetColorCommand = new DelegateCommand<Color>(SetColor);

            SetColor(Colors.Firebrick);
        }

        private void SetColor(Color color)
        {
            this.ForegroundColor = new SolidColorBrush(color) { Opacity = 1 };
        }

        public Brush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (Equals(value, _foregroundColor)) return;
                _foregroundColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand SetColorCommand { get; set; }

        private void Restart(MainModel model)
        {
            model.EndTime = DateTime.UtcNow.AddMinutes(25);
            model._timer.Start();
            Step();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e) => Step();

        private void Step()
        {
            var left = TimeLeft;
            if (left > TimeSpan.Zero) _timer.Start();
            OnPropertyChanged(nameof(TimeLeft));
        }

        public TimeSpan TimeLeft => EndTime - DateTime.UtcNow;
        public ICommand RestartCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T variable)
                _action(variable);
        }

        public event EventHandler CanExecuteChanged;
    }
}