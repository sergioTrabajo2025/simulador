using System;

namespace wpf1erprograma.Domain
{
    public interface IDigitalOutputService : IDisposable
    {
        int LineCount { get; }
        bool[] Snapshot();          // estado actual (copia)
        void WriteLine(int index, bool value);
        void WriteAll(bool value);
    }
}
