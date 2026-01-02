using System;
using System.Collections.ObjectModel;

namespace EarTrumpet.DataModel.Audio
{
    public interface IAudioDeviceManager
    {
        event EventHandler<IAudioDevice> DefaultChanged;
        event EventHandler<IAudioDevice> DefaultCommunicationsChanged;
        event EventHandler Loaded;
        IAudioDevice Default { get; set; }
        IAudioDevice DefaultCommunications { get; }
        ObservableCollection<IAudioDevice> Devices { get; }
        string Kind { get; }
        void UpdatePeakValues();
        void AddFilter(Func<ObservableCollection<IAudioDevice>, ObservableCollection<IAudioDevice>> filter);
    }
}