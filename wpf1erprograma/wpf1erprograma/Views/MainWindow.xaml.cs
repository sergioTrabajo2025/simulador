using System;
using System.Windows;
using wpf1erprograma.Infrastructure;
using wpf1erprograma.ViewModels;

namespace wpf1erprograma.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            var channel =
              "cDAQ9189-1E05111Mod1/port0/line0:7";

            var io = new NiDaqDigitalOutputService(channel, 8);

            _vm = new MainViewModel(io);
            DataContext = _vm;

            Closed += (_, __) => _vm.Dispose();
        }
    }
}
