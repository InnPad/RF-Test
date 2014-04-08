using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;

namespace Lucas
{
    using Lucas.Drawing;
    using System.Net;
    using System.Net.Sockets;

    partial class Program
    {
        private readonly Thread _thread;
        private readonly MainWindow _window;
        private NamedPipeClientStream _pipeStream;
        private StreamReader _pipeReader;
        private readonly Canvas _canvas;
        private bool _continue = true;
        private Dictionary<string, Action<string[]>> _commands;

        public Program(MainWindow window)
        {
            _window = window;
            _canvas = new Canvas();
            _commands = new Dictionary<string, Action<string[]>>();

            InitializeCommands();
            InitializeShapes();

            try
            {
                var ip = (Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? (object)string.Empty).ToString();
                _pipeStream = new NamedPipeClientStream(ip, "datapipe", PipeDirection.InOut, PipeOptions.WriteThrough, TokenImpersonationLevel.Impersonation);
                _pipeReader = new StreamReader(_pipeStream);
                _pipeStream.InitializeLifetimeService();
            }
            catch (Exception e)
            {
            }

            _thread = new Thread(MessageLoop);
            _thread.Start();
        }

        public Canvas Canvas { get { return _canvas; } }

        public void Exit()
        {
            _continue = false;
        }

        void MessageLoop()
        {
            _window.SetStatus("Idle");
            _window.SetMessage("Waiting for connection...");

            for (; _continue; )
            {
                try
                {
                    if (!_pipeStream.IsConnected)
                    {

                        _pipeStream.Connect(100);
                        if (_pipeStream.IsConnected)
                        {
                            _window.SetStatus("Connected");
                            _window.SetMessage("Waiting for input...");
                        }
                        continue;
                    }

                    string input = _pipeReader.ReadLine();

                    _window.SetStatus("Reveived");
                    _window.SetMessage(input);

                    if (input == null)
                    {
                        Thread.SpinWait(10);
                        continue;
                    }

                    KeyValuePair<int?, IShape> shape;
                    if (_canvas.TryParse(input, out shape))
                    {
                        if (shape.Key.HasValue)
                            _canvas[shape.Key.Value] = shape.Value;
                        else
                            _canvas.Add(shape.Value);

                        _window.Add(shape.Key ?? new Random().Next(), shape.Value);
                    }
                    else
                    {
                        var args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (string.Equals("exit", args[0], StringComparison.OrdinalIgnoreCase))
                            break;

                        if (string.Equals("clear", args[0], StringComparison.OrdinalIgnoreCase))
                        {
                            _window.Clear();
                        }

                        if (string.Equals("delete", args[0], StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var arg in args.Skip(1))
                            {
                                int key;
                                if (int.TryParse(arg, out key))
                                    _window.Remove(key);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    _window.SetStatus("Error");
                    _window.SetMessage(e.Message.Trim());
                }
                finally
                {
                    Thread.SpinWait(10);
                }
            }

            _window.Dispatcher.BeginInvoke((Action)_window.Close);
        }
    }
}
