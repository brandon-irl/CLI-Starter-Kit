using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLIStarterKit.Commands
{
	public abstract class BaseConsoleCommand
	{
		public string Name { get; set; }
		public string LibraryClassName { get; set; }
		public IEnumerable<string> Arguments { get; private set; }

		public BaseConsoleCommand() { }

		public BaseConsoleCommand(string arguments)
		{
			this.ParseArgumentString(arguments);
		}

		private void ParseArgumentString(string arguments)
		{
			var parsedArguments = new List<string>();
			var args = Regex.Split(
				arguments,
				"(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

			var isFirst = true;
			foreach (var str in args)
			{
				if (isFirst)
				{
					this.Name = str;
					this.LibraryClassName = "DefaultCommands";

					var parameters = str.Split('.');
					if (parameters.Length > 1)
					{
						this.LibraryClassName = parameters[0];
						this.Name = parameters[1];
					}

					isFirst = false;
				}
				else
				{
					parsedArguments.Add(str);
					//var regex = new Regex("\"(.*?)\"", RegexOptions.Singleline);
					//var match = regex.Match(str);

					//if (match.Captures.Count > 0)
					//{
					//    // Get the unquoted text:
					//    var captureQuotedText = new Regex("[^\"]*[^\"]");
					//    var quoted = captureQuotedText.Match(match.Captures[0].Value);
					//    parsedArguments.Add(quoted.Captures[0].Value);
					//}
				}
			}

			this.Arguments = parsedArguments;
		}
	}

	internal class ConsoleCommand : BaseConsoleCommand
	{
		public ConsoleCommand(string arguments) : base(arguments)
		{
		}
	}

	static class DefaultCommands
	{
		#region Test Junk
		public static string DoSomething(int id, string data)
		{
			return $"I did something to the record id {id} and save the data {data}";
		}

		public static string DoSomethingElse(DateTime date)
		{
			return $"I did something else on {date}";
		}

		public static string DoSomethingOptional(int id, string data = "No Data Provided")
		{
			var result = string.Format(
				"I did something to the record Id {0} and save the data {1}", id, data);

			if (data == "No Data Provided")
			{
				result = string.Format(
				"I did something to the record Id {0} but the optinal parameter "
				+ "was not provided, so I saved the value '{1}'", id, data);
			}
			return result;
		}
		#endregion
	}
}
