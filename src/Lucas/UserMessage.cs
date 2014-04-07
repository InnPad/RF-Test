using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucas
{
    internal struct UserMessage
    {
        private readonly UserCommands _command;
        private readonly string[] _arguments;

        public UserMessage(UserCommands cmd, params string[] args)
        {
            _command = cmd;
            _arguments = args;
        }

        public UserCommands Command { get { return _command; } }

        public string[] Arguments { get { return _arguments; } }

        public static UserMessage Parse(string s)
        {
            UserMessage result;
            UserCommands cmd;
            double value;
            
            var tokens = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
            {
                result = new UserMessage(UserCommands.Empty, new string[0]);
            }
            else if (!double.TryParse(tokens[0], out value) && Enum.TryParse<UserCommands>(tokens[0], true, out cmd))
            {
                result = cmd > UserCommands.Unknown
                    ? new UserMessage(cmd, tokens.Skip(1).ToArray())
                    : new UserMessage(UserCommands.Unknown, tokens);
            }
            else
            {
                result = new UserMessage(UserCommands.Unknown, tokens);
            }

            return result;
        }
    }
}
