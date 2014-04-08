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
            Register(keyword: "clear", perform: (string[] args) => { _window.Clear(); _canvas.Clear(); });

            Register(
                keyword: "delete",
                perform: (string[] args) =>
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        int key;
                        if (!args[i].TryParse(out key))
                            continue;

                        _window.Remove(key);
                    }
                });

            Register(keyword: "exit", perform: (string[] args) => _continue = false);
        }

        void Register(string keyword, Action<string[]> perform)
        {
            _commands[keyword.ToLower()] = perform;
        }
    }
}
