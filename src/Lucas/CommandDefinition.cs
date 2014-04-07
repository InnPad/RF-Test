using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    public struct CommandDefinition
    {
        public Action<string[]> Perform { get; set; }

        public Func<string> Syntax { get; set; }

        public Func<string> Summary { get; set; }

        public Func<string> Example { get; set; }
    }
}
