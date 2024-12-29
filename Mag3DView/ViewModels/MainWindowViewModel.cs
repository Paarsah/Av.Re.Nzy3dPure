using ReactiveUI;
using System.Reactive;

namespace Mag3DView.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        private double _rotationAngle;
        public double RotationAngle
        {
            get => _rotationAngle;
            set => this.RaiseAndSetIfChanged(ref _rotationAngle, value);
        }

        public ReactiveCommand<Unit, Unit> ResetViewCommand { get; }

        public MainWindowViewModel()
        {
            ResetViewCommand = ReactiveCommand.Create(() =>
            {
                RotationAngle = 0;
            });
        }
    }
}
