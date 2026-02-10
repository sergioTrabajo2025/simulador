using System;
using System.Linq;
using NationalInstruments.DAQmx;
using DaqTask = NationalInstruments.DAQmx.Task;
using wpf1erprograma.Domain;

namespace wpf1erprograma.Infrastructure
{
    public sealed class NiDaqDigitalOutputService : IDigitalOutputService
    {
        private readonly DaqTask _task;
        private readonly DigitalSingleChannelWriter _writer;
        private readonly bool[] _state;

        public int LineCount => _state.Length;

        public NiDaqDigitalOutputService(string channel, int lineCount)
        {
            _state = new bool[lineCount];

            _task = new DaqTask();
            _task.DOChannels.CreateChannel(
                channel,
                "DO_ALL",
                ChannelLineGrouping.OneChannelForAllLines);

            _writer = new DigitalSingleChannelWriter(_task.Stream);

            // arranca todo OFF
            TryWrite();
        }

        public void WriteLine(int index, bool value)
        {
            if (index < 0 || index >= _state.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            _state[index] = value;
            TryWrite();
        }

        public void WriteAll(bool value)
        {
            for (int i = 0; i < _state.Length; i++)
                _state[i] = value;

            TryWrite();
        }

        public bool[] Snapshot() => (bool[])_state.Clone();

        private void TryWrite()
        {
            // DAQmx: siempre se escribe el vector completo
            _writer.WriteSingleSampleMultiLine(true, _state);
        }

        public void Dispose()
        {
            try
            {
                // apagar todo al salir
                for (int i = 0; i < _state.Length; i++)
                    _state[i] = false;

                TryWrite();
            }
            catch { }

            _task?.Dispose();
        }
    }
}
