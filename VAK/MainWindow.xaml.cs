using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NAudio.Wave;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using VAK.Types;

namespace VAK
{
    public sealed partial class MainWindow : Window
    {
        private VoiceActivation? voiceActivation;

        private readonly List<WaveInCapabilities> MicList = Enumerable.Range(0, WaveIn.DeviceCount).Select(e => WaveIn.GetCapabilities(e)).ToList();

        private IEnumerable<Process> processes;
        private ObservableCollection<ProcessInfo> Processes { get; set; } = [];

        public MainWindow()
        {
            processes = Process.GetProcesses().Where(e => e.MainWindowHandle > 0); //ne récupère que les process ayant une fenêtre

            foreach (var process in processes)
                Processes.Add(new ProcessInfo(process));

            InitializeComponent();
        }

        private void VoiceActivationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn)
                {
                    char toSend = keyToSend.Text.Length != 0 ? keyToSend.Text.First() : 'A';

                    WaveInCapabilities mic;
                    var selectedMic = MicrophonesList.SelectedItem;
                    if (selectedMic != null)
                        mic = (WaveInCapabilities)selectedMic;
                    else
                        mic = MicList.First();

                    var selectedProcess = (ProcessInfo)ProcessList.SelectedItem;

                    voiceActivation = new VoiceActivation((short)MicList.IndexOf(mic), selectedProcess);
                    voiceActivation.Start(KeyboardInputHelper.CharToHex(toSend));
                }
                else
                {
                    voiceActivation?.Stop();
                }
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            processes = Process.GetProcessesByName(processSearch.Text).Where(e => e.MainWindowHandle > 0);

            ProcessList.SelectedItem = null;

            Processes.Clear();
            foreach (var process in processes)
                Processes.Add(new ProcessInfo(process));

            ProcessList.SelectedItem = Processes.FirstOrDefault();
        }
    }
}
