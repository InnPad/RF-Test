using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    using Lucas.Drawing;
    using Lucas.Drawing.Shapes;

    partial class Program
    {
        void InitializeCommands()
        {
            Register(
                keyword: "count",
                syntax: () => Strings.CMD_COUNT_SYNTAX,
                summary: () => Strings.CMD_COUNT_SUMMARY,
                example: () => Strings.CMD_COUNT_EXAMPLE,
                perform: (string[] args) =>
                {
                    if (args.Length > 0)
                    {
                        var count = _canvas.Keys.Select(key => new { Key = key, Value = _canvas[key] }).Where(o => o.Value != null && args.Contains(o.Value.GetType().Name.ToLower())).Count();
                        Console.WriteLine(Strings.SHAPE_COUNT_FORMAT, count);
                    }
                    else
                    {
                        Console.WriteLine(Strings.SHAPE_COUNT_FORMAT, _canvas.Count);
                    }
                });

            Register(
                keyword: "clear",
                syntax: () => Strings.CMD_CLEAR_SYNTAX,
                summary: () => Strings.CMD_CLEAR_SUMMARY,
                example: () => Strings.CMD_CLEAR_EXAMPLE,
                perform: (string[] args) =>
                {
                    args.Length
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(0, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "clear", 0, args.Length));

                    _pipeWriter.WriteLine("clear");
                    _pipeWriter.Flush();

                    Console.Clear();
                });

            Register(
                keyword: "delete",
                syntax: () => Strings.CMD_DELETE_SYNTAX,
                summary: () => Strings.CMD_DELETE_SUMMARY,
                example: () => Strings.CMD_DELETE_EXAMPLE,
                perform: (string[] args) =>
                {
                    (args.Length > 0)
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(true, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "delete", 1, args.Length));

                    if (args.Length.Equals(1) && args[0].Equals("all"))
                    {
                        _canvas.Clear();
                    }
                    else
                    {
                        for (var i = 0; i < args.Length; i++)
                        {
                            int key;
                            if (!args[i].TryParse(out key))
                            {
                                // Evaluate precondition. All arguments must be a double type
                                Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_ARGUMENT_TYPE, "integer", args[i], i);
                                continue;
                            }

                            var shape = _canvas.Remove(key);

                            _pipeWriter.WriteLine("delete {0}", key);
                            _pipeWriter.Flush();

                            if (shape != null)
                            {
                                Console.WriteLine(Strings.CMD_DELETE_SUCCESS, key, shape);
                            }
                            else
                            {
                                Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_SHAPE_NOT_FOUND, key);
                            }
                        }
                    }
                });

            Register(
                keyword: "draw",
                syntax: () => Strings.CMD_DRAW_SYNTAX,
                summary: () => Strings.CMD_DRAW_SUMMARY,
                example: () => Strings.CMD_DRAW_EXAMPLE,
                perform: (string[] args) =>
                {
                    (args.Length > 0)
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(true, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "draw", 1, args.Length));

                    for (var i = 0; i < args.Length; i++)
                    {
                        int key;

                        if (args[i].ToLower().Equals("auto"))
                        {
                            _autodraw = true;
                        }
                        else if (args[i].ToLower().Equals("off"))
                        {
                            _autodraw = false;
                        }
                        else if (int.TryParse(args[i], out key))
                        {
                            var shape = _canvas[key];

                            if (shape == null)
                            {
                                Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_SHAPE_NOT_FOUND, key);
                                continue;
                            }

                            Console.Write(Strings.SHAPE_PRINT, key);
                            Console.WriteLine(shape.Print());

                            if (_autodraw)
                            {
                                _pipeWriter.WriteLine(string.Join(" ", key, shape.Stringify()));
                                _pipeWriter.Flush();
                            }
                        }
                        else
                        {
                            Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_ARGUMENT_TYPE, "integer", args[i], i);
                        }
                    }
                });

            Register(
                keyword: "exit",
                syntax: () => Strings.CMD_EXIT_SYNTAXIS,
                summary: () => Strings.CMD_EXIT_SUMMARY,
                example: () => Strings.CMD_EXIT_EXAMPLE,
                perform: (string[] args) =>
                {
                    _continue = false;
                    _pipeWriter.WriteLine("exit");
                    _pipeWriter.Flush();
                    Console.Clear();
                });

            Register(
                keyword: "help",
                syntax: () => Strings.CMD_HELP_SYNTAX,
                summary: () => Strings.CMD_HELP_SUMMARY,
                example: () => Strings.CMD_HELP_EXAMPLE,
                perform: (string[] args) =>
                {
                    Action<string, ShapeDefinition> PrintShapeDefinition = (string keyword, ShapeDefinition shape) =>
                        Console.WriteLine(string.Format(Strings.HELP_SHAPE_DEFINITION_FORMAT, keyword, shape.Summary(), shape.Example()).Replace("\\n", "\r\n").Replace("\\t", "    "));

                    Action<string, CommandDefinition> PrintCommandDefinition = (string keyword, CommandDefinition cmd) =>
                        Console.WriteLine(string.Format(Strings.HELP_CMD_DEFINITION_FORMAT, keyword, cmd.Syntax(), cmd.Summary(), cmd.Example()).Replace("\\n", "\r\n").Replace("\\t", "    "));

                    if (args.Length > 0)
                    {
                        foreach (var arg in args)
                        {
                            CommandDefinition cmd;
                            if (_commands.TryGetValue(arg.ToLower(), out cmd))
                            {
                                Console.WriteLine();
                                PrintCommandDefinition(arg.ToLower(), cmd);
                            }
                            else
                            {
                                var shape = _canvas.Definitions.SingleOrDefault(o => o.Key == arg.ToLower());

                                if (shape.Value != null)
                                {
                                    Console.WriteLine();
                                    PrintShapeDefinition(shape.Key, shape.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var cmd in _commands)
                        {
                            Console.WriteLine();
                            PrintCommandDefinition(cmd.Key, cmd.Value);
                        }

                        foreach (var shape in _canvas.Definitions)
                        {
                            Console.WriteLine();
                            PrintShapeDefinition(shape.Key, shape.Value);
                        }
                    }
                });

            Register(
                keyword: "list",
                syntax: () => Strings.CMD_LIST_SYNTAX,
                summary: () => Strings.CMD_LIST_SUMMARY,
                example: () => Strings.CMD_LIST_EXAMPLE,
                perform: (string[] args) =>
                {
                    if (args.Length > 0)
                    {
                        if (_autodraw)
                        {
                            _pipeWriter.WriteLine("clear");
                            _pipeWriter.Flush();
                        }
                        var matches = _canvas.Keys.Select(key => new { Key = key, Value = _canvas[key] }).Where(o => o.Value != null && args.Contains(o.Value.GetType().Name.ToLower())).ToArray();
                        Console.WriteLine(Strings.SHAPE_COUNT_FORMAT, matches.Length);
                        foreach (var shape in matches)
                        {
                            Console.Write(Strings.SHAPE_PRINT, shape.Key);
                            Console.WriteLine(shape.Value.Print());
                            if (_autodraw)
                            {
                                _pipeWriter.WriteLine(string.Join(" ", shape.Key, shape.Value.Stringify()));
                                _pipeWriter.Flush();
                            }
                        }
                    }
                    else
                    {
                        if (_autodraw)
                        {
                            _pipeWriter.WriteLine("clear");
                            _pipeWriter.Flush();
                        }
                        Console.WriteLine(Strings.SHAPE_COUNT_FORMAT, _canvas.Count);
                        foreach (var key in _canvas.Keys)
                        {
                            var shape = _canvas[key];
                            Console.Write(Strings.SHAPE_PRINT, key);
                            Console.WriteLine(shape.Print());

                            if (_autodraw)
                            {
                                _pipeWriter.WriteLine(string.Join(" ", key, shape.Stringify()));
                                _pipeWriter.Flush();
                            }
                        }
                    }
                });

            Register(
                keyword: "load",
                syntax: () => Strings.CMD_LOAD_SYNTAX,
                summary: () => Strings.CMD_LOAD_SUMMARY,
                example: () => Strings.CMD_LOAD_EXAMPLE,
                perform: (string[] args) =>
                {
                    (args.Length > 0)
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(true, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "load", 1, args.Length));

                    foreach (var arg in args)
                    {
                        var path = Path.GetFullPath(arg);

                        if (File.Exists(path))
                        {
                            Load(File.Open(path, FileMode.Open));
                        }
                        else
                        {
                            Console.WriteLine(Strings.ERROR_PRINT + Strings.ERROR_FILE_NOT_FOUND, path);
                        }
                    }
                });

            Register(
                keyword: "over",
                syntax: () => Strings.CMD_OVER_SYNTAX,
                summary: () => Strings.CMD_OVER_SUMMARY,
                example: () => Strings.CMD_OVER_EXAMPLE,
                perform: (string[] args) =>
                {
                    args.Length
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(1, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "over", 1, args.Length));

                    int key;

                    args
                        .First()
                        .TryParse(out key)
                        // Evaluate precondition. All arguments must be a double type
                        .Assert(true, () => string.Format(Strings.ERROR_ARGUMENT_TYPE, "integer", args, 0));

                    var shape = _canvas[key];

                    (shape != null)
                        .Assert(true, () => string.Format(Strings.ERROR_SHAPE_NOT_FOUND, key));

                    if (_autodraw)
                    {
                        _pipeWriter.WriteLine(string.Join(" ", key, shape.Stringify()));
                        _pipeWriter.Flush();
                    }

                    if (_autodraw)
                    {
                        _pipeWriter.WriteLine("clear");
                        _pipeWriter.Flush();
                    }

                    Console.WriteLine(Strings.LISTING_SHAPES_OVERLAPING_SHAPE, key, shape);

                    foreach (var overlap in _canvas.Elements.Where(o => o.Key != key && o.Value != null && o.Value.Intersects(shape)))
                    {
                        Console.Write(Strings.SHAPE_PRINT, overlap.Key);
                        Console.WriteLine(overlap.Value.ToString());
                        if (_autodraw)
                        {
                            _pipeWriter.WriteLine(string.Join(" ", overlap.Key, overlap.Value.Stringify()));
                            _pipeWriter.Flush();
                        }
                    }
                });

            Register(
                keyword: "save",
                syntax: () => Strings.CMD_SAVE_SYNTAX,
                summary: () => Strings.CMD_SAVE_SUMMARY,
                example: () => Strings.CMD_SAVE_EXAMPLE,
                perform: (string[] args) =>
                {
                    args.Length
                        // Evaluate precondition. A circle must be contructed from 3 arguments
                        .Assert(1, () => string.Format(Strings.ERROR_ARGUMENT_COUNT, "save", 1, args.Length));

                    var path = Path.GetFullPath(args[0]);

                    if (!File.Exists(path))
                    {
                        Save(File.Create(path));
                    }
                    else
                    {
                        Console.Write(Strings.PROMPT_FILE_OVERWRITE, path);

                        bool retry;

                        do
                        {
                            var ch = char.ToLower(Console.ReadKey().KeyChar);

                            switch (ch)
                            {
                                case 'y':
                                    Console.WriteLine();
                                    Save(File.Open(path, FileMode.Truncate));
                                    retry = false;
                                    break;

                                case 'n':
                                    Console.WriteLine();
                                    retry = false;
                                    break;

                                default:
                                    retry = true;
                                    break;
                            }
                        }
                        while (retry);
                    }
                });
        }

        void Register(string keyword, Func<string> syntax, Func<string> summary, Func<string> example, Action<string[]> perform)
        {
            _commands[keyword.ToLower()] = new CommandDefinition
            {
                Perform = perform,
                Syntax = syntax,
                Summary = summary,
                Example = example
            };
        }
    }
}
