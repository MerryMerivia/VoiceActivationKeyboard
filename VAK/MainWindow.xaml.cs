using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace VAK
{
    public sealed partial class MainWindow : Window
    {
        private VoiceActivation? voiceActivation;

        private readonly List<WaveInCapabilities> MicList = Enumerable.Range(0, WaveIn.DeviceCount).Select(e => WaveIn.GetCapabilities(e)).ToList();

        public MainWindow() => InitializeComponent();

        private void VoiceActivationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn)
                {
                    char toSend = keyToSend.Text.Length != 0 ? keyToSend.Text.First() : 'A';

                    WaveInCapabilities mic;
                    var selectedItem = MicrophonesList.SelectedItem;
                    if (selectedItem != null)
                        mic = (WaveInCapabilities)selectedItem;
                    else
                        mic = MicList.First();

                    voiceActivation = new VoiceActivation((short)(MicList.IndexOf(mic) - 1));
                    voiceActivation.Start(KeyboardInputHelper.CharToHex(toSend));
                }
                else
                {
                    voiceActivation?.Stop();
                }
            }
        }
    }
}
