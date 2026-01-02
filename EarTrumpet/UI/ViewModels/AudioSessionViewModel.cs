using EarTrumpet.DataModel.Audio;
using EarTrumpet.Extensions;
using EarTrumpet.UI.Helpers;
using System;
using System.Windows.Input;

namespace EarTrumpet.UI.ViewModels
{
    public class AudioSessionViewModel : BindableBase
    {
        private readonly IStreamWithVolumeControl _stream;
        private bool _isAbsMuted;

        // Interpolation state
        private float _prevPeakValue1;
        private float _prevPeakValue2;
        private float _targetPeakValue1;
        private float _targetPeakValue2;
        private float _displayPeakValue1;
        private float _displayPeakValue2;
        private int _interpolationStep;
        private int _interpolationSteps;

        public AudioSessionViewModel(IStreamWithVolumeControl stream)
        {
            _stream = stream;
            _stream.PropertyChanged += Stream_PropertyChanged;

            _isAbsMuted = false;
            _interpolationStep = 0;
            _interpolationSteps = 1;

            ToggleMute = new RelayCommand(() => IsMuted = !IsMuted);
        }

        ~AudioSessionViewModel()
        {
            _stream.PropertyChanged -= Stream_PropertyChanged;
        }

        private void Stream_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        public string Id => _stream.Id;
        public ICommand ToggleMute { get; }
        public bool IsMuted
        {
            get => _stream.IsMuted;
            set => _stream.IsMuted = value;
        }

        public bool IsAbsMuted
        {
            get => _isAbsMuted;
            set => _isAbsMuted = value;
        }

        public int Volume
        {
            get => _stream.Volume.ToVolumeInt();
            set => _stream.Volume = value / 100f;
        }
        public virtual float PeakValue1 => _displayPeakValue1;
        public virtual float PeakValue2 => _displayPeakValue2;

        public virtual void OnNewSample(int interpolationSteps)
        {
            // Current display position becomes the new "previous" position
            _prevPeakValue1 = _displayPeakValue1;
            _prevPeakValue2 = _displayPeakValue2;

            // New sample becomes the target
            _targetPeakValue1 = _stream.PeakValue1;
            _targetPeakValue2 = _stream.PeakValue2;

            // Reset interpolation
            _interpolationStep = 0;
            _interpolationSteps = Math.Max(1, interpolationSteps);
        }

        public virtual void UpdatePeakValueForeground()
        {
            _interpolationStep++;

            if (_interpolationStep >= _interpolationSteps)
            {
                // We've reached or passed the target, snap to it
                _displayPeakValue1 = _targetPeakValue1;
                _displayPeakValue2 = _targetPeakValue2;
            }
            else
            {
                // Linear interpolation: lerp from prev to target
                float t = (float)_interpolationStep / _interpolationSteps;
                _displayPeakValue1 = _prevPeakValue1 + (_targetPeakValue1 - _prevPeakValue1) * t;
                _displayPeakValue2 = _prevPeakValue2 + (_targetPeakValue2 - _prevPeakValue2) * t;
            }

            RaisePropertyChanged(nameof(PeakValue1));
            RaisePropertyChanged(nameof(PeakValue2));
        }
    }
}
