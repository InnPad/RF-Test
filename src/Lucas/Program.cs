using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lucas
{
    using Lucas.Drawing;
    using Lucas.Drawing.Shapes;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.AccessControl;
    using System.Security.Principal;

    partial class Program
    {
        private readonly Canvas _canvas;
        private NamedPipeServerStream _pipeStream;
        private StreamWriter _pipeWriter;
        private bool _autodraw = false;
        private bool _continue = true;
        private Dictionary<string, CommandDefinition> _commands;

        static void Main(string[] args)
        {
            var program = new Program();

            program.InitializeCommands();
            program.InitializeShapes();

            foreach (var filename in args)
            {
                if (File.Exists(filename))
                {
                    continue;
                }

                var path = Path.GetFullPath(filename);

                if (File.Exists(path))
                {
                    program.Load(File.Open(path, FileMode.Open));
                }
                else
                {
                    Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_FILE_NOT_FOUND, path);
                }
            }

            program.InputLoop();
        }

        private Program()
        {
            PipeSecurity ps = new PipeSecurity();
            //ps.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().Owner, PipeAccessRights.FullControl, AccessControlType.Allow));
            ps.AddAccessRule(new PipeAccessRule(@"Everyone", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
            //ps.AddAccessRule(new PipeAccessRule("CREATOR OWNER", PipeAccessRights.FullControl, AccessControlType.Allow));
            //ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));
            //ps.AddAccessRule(pa);
            _pipeStream = new NamedPipeServerStream("datapipe", PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.WriteThrough, 1024, 1024, ps);
            _pipeWriter = new StreamWriter(_pipeStream);

            Console.WriteLine(Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork));

            var thread = new Thread(() =>
            {
                for (; _continue; )
                    try
                    {
                        if (_pipeStream.IsConnected)
                            Thread.SpinWait(100);
                        else
                            _pipeStream.WaitForConnection();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(Strings.ERROR_PRINT + e.Message);
                        _pipeStream = new NamedPipeServerStream("datapipe", PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.WriteThrough, 1024, 1024, ps);
                        _pipeWriter = new StreamWriter(_pipeStream);
                    }
            });
            thread.Start();

            _autodraw = false;
            _continue = true;
            _canvas = new Canvas();
            _commands = new Dictionary<string, CommandDefinition>();
        }

        void InputLoop()
        {
            Point point;

            Console.Write(Strings.PROMPT);

            do
            {
                var input = Console.ReadLine();

                try
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                    }
                    else if (TryPerform(input))
                    {
                    }
                    else if (LoadShape(input))
                    {
                    }
                    else if (Point.TryParse(input, out point))
                    {
                        Console.WriteLine(Strings.LISTING_SHAPES_CONTAINING_POINT, point.X, point.Y);

                        if (_autodraw)
                        {
                            _pipeWriter.Write("clear");
                            _pipeWriter.Flush();
                        }

                        foreach (var overlap in _canvas.Elements.Where(o => o.Value != null && o.Value.Contains(point)))
                        {
                            Console.Write(Strings.SHAPE_PRINT, overlap.Key);
                            Console.WriteLine(overlap.Value.ToString());

                            if (_autodraw)
                            {
                                _pipeWriter.WriteLine(string.Join(" ", overlap.Key, overlap.Value.Stringify()));
                                _pipeWriter.Flush();
                            }
                        }
                    }
                    else
                    {
                        false.Assert(true, () => Strings.ERROR_UNKNOW_REQUEST);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(Strings.ERROR_PRINT + (e.InnerException != null ? e.InnerException.Message : e.Message));
                }
                finally
                {
                    Console.WriteLine();
                    Console.Write(Strings.PROMPT);
                }
            }
            while (_continue);
        }

        void Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    LoadShape(line);
                }
            }
        }

        bool LoadShape(string s)
        {
            KeyValuePair<int?, IShape> result;
            if (_canvas.TryParse(s, out result))
            {
                int key;

                if (result.Key.HasValue)
                {
                    key = result.Key.Value;
                    _canvas[key] = result.Value;
                }
                else
                {
                    key = _canvas.Add(result.Value);
                }

                Console.Write(Strings.SHAPE_PRINT, key);
                Console.WriteLine(result.Value.Print());
                _pipeWriter.WriteLine(string.Join(" ", key, result.Value.Stringify()));
                _pipeWriter.Flush();

                return true;
            }

            return false;
        }

        void Save(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                var keys = _canvas.Keys;
                Array.Sort(keys);
                foreach (var key in keys)
                {
                    var shape = _canvas[key];
                    var s = shape.Stringify();
                    if (s != null)
                    {
                        writer.WriteLine(string.Join(" ", key, s));
                    }
                }
            }
        }

        bool TryPerform(string s)
        {
            var args = s.Split(new[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);

            CommandDefinition cmd;
            if (_commands.TryGetValue(args[0].ToLower(), out cmd))
            {
                var action = cmd.Perform;

                if (action != null)
                {
                    action(args.Skip(1).ToArray());
                }

                return true;
            }

            return false;
        }
    }
}
