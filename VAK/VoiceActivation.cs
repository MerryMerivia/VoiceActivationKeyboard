using NAudio.Wave;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VAK.Types;

namespace VAK
{
    internal partial class VoiceActivation : IDisposable
    {
        /// <summary>
        /// Fonction d'envoi d'événement clavier
        /// </summary>
        /// <param name="bVk">Code héxadécimal représentant de la touche concercée</param>
        /// <param name="bScan">?</param>
        /// <param name="dwFlags">Code héxadécimal réprésentant le type d'event</param>
        /// <param name="dwExtraInfo">?</param>
        [LibraryImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        private static partial void KeyboardEvent(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [LibraryImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        private static partial IntPtr GetForegroundWindow();

        private const int KEYEVENTF_KEYDOWN = 0x0000; // Appui sur une touche
        private const int KEYEVENTF_KEYUP = 0x0002;   // Relâchement d'une touche
        private const int KEYUP_MS_THRESHOLD = 500;

        private DateTime date = DateTime.Now.AddDays(-1);

        private readonly WaveInEvent waveInEvent;

        private readonly ProcessInfo? focusProcess;

        private Task? VoiceActivationTask;
        private CancellationTokenSource Source { get; set; } = new();
        private CancellationToken SourceToken => Source.Token;

        internal VoiceActivation(int deviceNumber = 0, ProcessInfo? processToFocus = null, int bufferMilliseconds = 20)
        {
            waveInEvent = new()
            {
                DeviceNumber = deviceNumber, // indicates which microphone to use
                WaveFormat = new WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = bufferMilliseconds
            };
            waveInEvent.DataAvailable += WaveIn_DataAvailable;

            focusProcess = processToFocus;
        }

        internal void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            // copy buffer into an array of integers
            short[] values = new short[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.Buffer.Length);

            // determine the highest value as a fraction of the maximum possible value
            float fraction = (float)values.Max() / 32768;

            Console.WriteLine(fraction);
            if (fraction > 0.1)
            {
                date = DateTime.Now;
            }
        }

        internal async void Start(byte configuredKey)
        {
            Source = new();

            VoiceActivationTask = Task.Run(async () =>
            {
                waveInEvent.StartRecording();
                bool pressed = false;

                while (true)
                {
                    if (focusProcess is not null)
                    {
                        var currentActiveWindowHandle = GetForegroundWindow();

                        if (focusProcess.MainWindowHandle != currentActiveWindowHandle)
                        {
                            await Task.Delay(300);
                            continue;
                        }

                    }

                    if (Math.Abs(DateTime.Now.Millisecond - date.Millisecond) > KEYUP_MS_THRESHOLD)
                    {
                        if (pressed)
                        {
                            pressed = false;
                            KeyboardEvent(configuredKey, 0, KEYEVENTF_KEYUP, 0);
                            Console.WriteLine(pressed);
                        }
                    }
                    else
                    {
                        if (!pressed)
                        {
                            pressed = true;
                            KeyboardEvent(configuredKey, 0, KEYEVENTF_KEYDOWN, 0);
                            Console.WriteLine(pressed);
                        }
                    }

                    if (SourceToken.IsCancellationRequested)
                    {
                        waveInEvent.StopRecording();
                        SourceToken.ThrowIfCancellationRequested();
                    }
                }
            }, SourceToken);

            try
            {
                await VoiceActivationTask;
            }
            catch (OperationCanceledException) { }
            finally
            {
                Source.Dispose();
            }
        }

        internal void Stop()
        {
            Source.Cancel();
        }

        public void Dispose()
        {
            Stop();
            waveInEvent.DataAvailable -= WaveIn_DataAvailable;
        }
    }
}
