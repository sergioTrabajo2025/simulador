using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using wpf1erprograma.Domain;

namespace wpf1erprograma.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDigitalOutputService _io;
        private bool _suppress;

        public ObservableCollection<LineVm> Lines { get; }
        public ICommand ToggleAllCommand { get; }

        private string _toggleAllText = "TODO ON";
        public string ToggleAllText
        {
            get => _toggleAllText;
            private set { _toggleAllText = value; OnPropertyChanged(nameof(ToggleAllText)); }
        }

        public MainViewModel(IDigitalOutputService io)
        {
            _io = io;

            // Orden visual: fila por fila (8x3) como tu UniformGrid
            // Poné acá exactamente tus labels en el orden que querés verlos.
            string[] labels =
            {
                "01","11","23",
                "12","22","23",
                "31","32","33",
                "41","42","43",
                "51","52","53",
                "61","62","63",
                "71","72","73",
                "81","82","83"
            };

            var snap = _io.Snapshot();

            Lines = new ObservableCollection<LineVm>(
                labels.Select((text, i) =>
                {
                    var vm = new LineVm(text, i) { IsOn = (i < snap.Length) && snap[i] };
                    vm.PropertyChanged += Line_PropertyChanged;
                    return vm;
                })
            );

            ToggleAllCommand = new RelayCommand(_ => ToggleAll());

            UpdateAllButtonText();
        }

        private void Line_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_suppress) return;
            if (e.PropertyName != nameof(LineVm.IsOn)) return;

            var line = (LineVm)sender!;
            _io.WriteLine(line.Index, line.IsOn);
            UpdateAllButtonText();
        }

        private void ToggleAll()
        {
            bool target = !Lines.All(l => l.IsOn);

            _io.WriteAll(target);

            _suppress = true;
            try
            {
                foreach (var l in Lines) l.IsOn = target;
            }
            finally { _suppress = false; }

            UpdateAllButtonText();
        }

        private void UpdateAllButtonText()
        {
            ToggleAllText = Lines.All(l => l.IsOn) ? "TODO OFF" : "TODO ON";
        }

        public void Dispose() => _io.Dispose();

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public sealed class LineVm : INotifyPropertyChanged
        {
            public string Label { get; }
            public int Index { get; }

            private bool _isOn;
            public bool IsOn
            {
                get => _isOn;
                set { _isOn = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOn))); }
            }

            public LineVm(string label, int index)
            {
                Label = label;
                Index = index;
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            public RelayCommand(Action<object?> execute) => _execute = execute;

            public bool CanExecute(object? parameter) => true;
            public void Execute(object? parameter) => _execute(parameter);
            public event EventHandler? CanExecuteChanged;
        }
    }
}
