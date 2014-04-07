using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public enum UserCommands : byte
    {
        Empty = 0,

        Unknown = 1,

        Call,

        Count,

        Delete,

        Display,

        Exit,

        Help,

        List,

        Load,

        Over,

        Save
    }
}
