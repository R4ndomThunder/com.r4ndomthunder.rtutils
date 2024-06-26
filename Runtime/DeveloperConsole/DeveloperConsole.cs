

using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
*   MIT License
*
*   Samuele Padalino @R4ndomThunder
*   https://samuelepadalino.dev
*/
namespace RTDK.DeveloperConsole
{
    public class DeveloperConsole
    {
        private readonly string prefix;
        private readonly IEnumerable<IConsoleCommand> commands;

        public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix)) { return; }

            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            ProcessCommand(commandInput, args);
        }

        public void ProcessCommand(string commandInput, string[] args)
        {
            foreach (var command in commands)
            {
                if (!commandInput.Equals(command.CommandWord, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (command.Process(args))
                    return;
            }
        }
    }
}