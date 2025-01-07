using ElectronCgi.DotNet;
using NAudio.Wave;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    internal static class Program
    {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_KEYDOWN = 0x0000; // Appui sur une touche
        private const int KEYEVENTF_KEYUP = 0x0002;   // Relâchement d'une touche
        private const int KEYUP_MS_THRESHOLD = 1000;

        
        private static DateTime date = DateTime.Now.AddDays(-1);

        private static WaveInEvent waveInEvent;

        private static Task VoiceActivationTask;
        private static CancellationTokenSource Source { get; set; } = new();
        private static CancellationToken SourceToken => Source.Token;

        static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // copy buffer into an array of integers
            short[] values = new short[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.Buffer.Length);

            // determine the highest value as a fraction of the maximum possible value
            float fraction = (float)values.Max() / 32768;

            Connection.Send("voice", fraction);

            if (fraction > 0.001)
            {
                date = DateTime.Now;
            }
        }

        static async void Start(byte configuredKey)
        {
            Source = new();

            VoiceActivationTask = Task.Run(() =>
            {
                waveInEvent.StartRecording();
                bool pressed = false;

                while (true)
                {
                    if (Math.Abs(DateTime.Now.Millisecond - date.Millisecond) > KEYUP_MS_THRESHOLD)
                    {
                        if (pressed)
                        {
                            pressed = false;
                            keybd_event(configuredKey, 0, KEYEVENTF_KEYUP, 0);
                        }
                    }
                    else
                    {
                        if (!pressed)
                        {
                            pressed = true;
                            keybd_event(configuredKey, 0, KEYEVENTF_KEYDOWN, 0);
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

        static Connection Connection;

        static void Main(string[] args)
        {
            int deviceNumber = 0; 
            int bufferMilliseconds = 20;

            waveInEvent = new()
            {
                DeviceNumber = deviceNumber, // indicates which microphone to use
                WaveFormat = new WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = bufferMilliseconds
            };
            waveInEvent.DataAvailable += WaveIn_DataAvailable;

            Connection = new ConnectionBuilder()
                .WithLogging()
                .Build();

            Start(0x42);  
            
            Connection.On<string, string>("greeting", name => "Hello " + name);
            
            Connection.Listen();  
        }
    }
}